using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;
using UniRx;
using UniRx.Triggers;

public class Cutscenes : MonoBehaviourSingleton<Cutscenes> {
    public YarnProject yarnProject;
    public VariableStorageBehaviour mainVariableStorage;
    public bool verboseLogging = false;
    public bool optionsAreDialogue = false;
    public DialogueViewBase[] dialogueViews;
    public Transform extensionContainer;
    public Transform runnerContainer;
    public LineProviderBehaviour lineProvider;

    List<OrangeYarnExtension> extensions;

    Queue<DialogueRunner> runnerPool = new Queue<DialogueRunner>();
    public int createdRunners = 0;

    public bool AreRunning => foregroundRunners.Count > 0;
    public BehaviorSubject<bool> foregroundRunningOb = new BehaviorSubject<bool>(false);

    protected override void Awake() {
        base.Awake();
        if (lineProvider != null) lineProvider.YarnProject = yarnProject;
        if (extensionContainer == null) extensionContainer = transform;
        extensions = extensionContainer.GetComponentsInChildren<OrangeYarnExtension>().ToList();
    }

    Dictionary<DialogueRunner, System.Action> dialogueDoneCallbacks = new Dictionary<DialogueRunner, System.Action>();
    HashSet<DialogueRunner> foregroundRunners = new HashSet<DialogueRunner>();


    [NaughtyAttributes.Button]
    void DoDebug() {
        Debug.Log($"foregroundRunners count={foregroundRunners.Count}");
        Debug.Log($"dialogueDoneCallbacks count={dialogueDoneCallbacks.Count}");
        Debug.Log($"runnerPool count={runnerPool.Count}");
    }

    bool canContinue = true;
    public void OnConfirm() {
        if (!canContinue) return;
        LineView lineView = dialogueViews.First(v => v is LineView) as LineView;

        canContinue = false;
        this.UpdateAsObservable().TakeUntilDestroy(this).Take(1).Subscribe(_ => {
            lineView.OnContinueClicked();
            canContinue = true;
        });
    }

    public StartedCutscene StartScript(string sceneName, System.Action onDone = null, bool runInBackground = false, Dictionary<string, object> context = null) {
        // Early out if no sceneName was passed
        if (string.IsNullOrWhiteSpace(sceneName)) {
            onDone?.Invoke();
            return null;
        }

        var (runner, didCreate) = ClaimRunner();
        SetRunnerContext(runner, context);
        runner.startAutomatically = didCreate;
        runner.startNode = didCreate ? sceneName : "";

        if (!runInBackground) {
            foregroundRunners.Add(runner);
            if (foregroundRunners.Count == 1) foregroundRunningOb.OnNext(true);
        }

        dialogueDoneCallbacks[runner] = onDone;
        // If this is a new runner we *have* to wait for Start() to finish initialization
        if (!didCreate) {
            try {
                runner.StartDialogue(sceneName);
            } catch (Yarn.DialogueException e) {
                Debug.LogError(e, this);
                HandleRunnerComplete(runner);
            }
        }

        return new StartedCutscene() { runner = runner, isRunning = true };
    }

    void SetRunnerContext(DialogueRunner runner, Dictionary<string, object> context) {
        if (context == null) return;
        var scopedVariables = runner.GetComponent<ScopedVariableStore>();
        foreach (var kv in context) {
            if (kv.Value.GetType() == typeof(int)) {
                scopedVariables.scopedData.SetValue($"$_{kv.Key}", (int)kv.Value);
            } else if (kv.Value.GetType() == typeof(float)) {
                scopedVariables.scopedData.SetValue($"$_{kv.Key}", (float)kv.Value);
            } else if (kv.Value.GetType() == typeof(bool)) {
                scopedVariables.scopedData.SetValue($"$_{kv.Key}", (bool)kv.Value);
            } else if (kv.Value.GetType() == typeof(string)) {
                scopedVariables.scopedData.SetValue($"$_{kv.Key}", (string)kv.Value);
            } else {
                // TODO: Handle null?
                Debug.LogError($"Invalid Type {kv.Value.GetType().ToString()} for key {kv.Key}!", runner);
            }
        }
    }

    (DialogueRunner, bool didCreate) ClaimRunner() {
        if (runnerPool.Count > 0) {
            return (runnerPool.Dequeue(), false);
        }

        var newRunnerObj = new GameObject($"Runner #{createdRunners}");
        newRunnerObj.transform.SetParent(runnerContainer);
        createdRunners += 1;
        var newRunner = newRunnerObj.AddComponent<DialogueRunner>();
        newRunner.startAutomatically = false;
        newRunner.startNode = "";
        newRunner.yarnProject = yarnProject;
        newRunner.onNodeComplete = new DialogueRunner.StringUnityEvent();
        newRunner.onNodeStart = new DialogueRunner.StringUnityEvent();
        newRunner.onDialogueComplete = new UnityEngine.Events.UnityEvent();
        newRunner.onCommand = new DialogueRunner.StringUnityEvent();
        newRunner.lineProvider = lineProvider;

        // Configure variable storage
        var scopedData = newRunnerObj.AddComponent<InMemoryVariableStorage>();
        var scopedVariableHandler = newRunnerObj.AddComponent<ScopedVariableStore>();
        scopedVariableHandler.prefix = "$_";
        scopedVariableHandler.scopedData = scopedData;
        scopedVariableHandler.parentData = mainVariableStorage;
        Destroy(newRunner.VariableStorage);
        newRunner.VariableStorage = scopedVariableHandler;
        newRunner.verboseLogging = verboseLogging;
        newRunner.runSelectedOptionAsLine = optionsAreDialogue;
        newRunner.SetDialogueViews(dialogueViews);
        newRunner.Dialogue.AddProgram(yarnProject.GetProgram());
        Destroy(newRunner.GetComponent<LineView>());

        // Register the extensions
        foreach (var extension in extensions) {
            extension.ConfigureRunner(newRunner);
        }

        // Register the done callback
        newRunner.onDialogueComplete = new UnityEngine.Events.UnityEvent();
        newRunner.onDialogueComplete.AddListener(() => {
            HandleRunnerComplete(newRunner);
        });
        return (newRunner, true);
    }

    void HandleRunnerComplete(DialogueRunner runner) {
        if (foregroundRunners.Contains(runner)) {
            foregroundRunners.Remove(runner);
            if (foregroundRunners.Count == 0) foregroundRunningOb.OnNext(false);
        }
        var action = dialogueDoneCallbacks[runner];
        dialogueDoneCallbacks.Remove(runner);
        runnerPool.Enqueue(runner);
        // TODO: Any other cleanup.
        action?.Invoke();
    }

    public static void UserRequestedViewAdvancement() {
        foreach (var dialogueView in Cutscenes.Instance.dialogueViews) {
            dialogueView.UserRequestedViewAdvancement();
            break;
        }
    }

    public class StartedCutscene {
        public bool isRunning = false;
        public DialogueRunner runner = null;
    }
}
