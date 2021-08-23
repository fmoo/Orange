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

    List<OrangeYarnExtension> extensions;

    Queue<DialogueRunner> runnerPool = new Queue<DialogueRunner>();
    public int createdRunners = 0;

    public bool AreRunning => foregroundRunners.Count > 0;

    protected override void Awake() {
        base.Awake();
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
        var lineView = GetComponentInChildren<LineView>();

        canContinue = false;
        this.UpdateAsObservable().TakeUntilDestroy(this).Take(1).Subscribe(_ => {
            lineView.OnContinueClicked();
            canContinue = true;
        });
    }

    public void StartScript(string sceneName, System.Action onDone = null, bool runInBackground = false) {
        var (runner, didCreate) = ClaimRunner();
        runner.startAutomatically = didCreate;
        runner.startNode = didCreate ? sceneName : "";

        if (!runInBackground) {
            foregroundRunners.Add(runner);
        }

        dialogueDoneCallbacks[runner] = onDone;
        // If this is a new runner we *have* to wait for Start() to finish initialization
        if (!didCreate) {
            runner.StartDialogue(sceneName);
        }

        // TODO: Return disposable / suspendable wrapper object.
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

        // Configure variable storage
        var scopedData = newRunnerObj.AddComponent<InMemoryVariableStorage>();
        var scopedVariableHandler = newRunnerObj.AddComponent<ScopedVariableStore>();
        scopedVariableHandler.prefix = "_";
        scopedVariableHandler.scopedData = scopedData;
        scopedVariableHandler.parentData = mainVariableStorage;
        Destroy(newRunner.variableStorage);
        newRunner.variableStorage = scopedVariableHandler;
        newRunner.verboseLogging = verboseLogging;
        newRunner.runSelectedOptionAsLine = optionsAreDialogue;
        newRunner.dialogueViews = dialogueViews.ToArray();
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
        }
        var action = dialogueDoneCallbacks[runner];
        dialogueDoneCallbacks.Remove(runner);
        runnerPool.Enqueue(runner);
        // TODO: Any other cleanup.
        action?.Invoke();
    }
}
