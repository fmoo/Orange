using Yarn.Unity;
using UniRx;
using UnityEngine;
using System.Collections.Generic;


public class OrangeCallScriptExtension : OrangeYarnExtension {
    public override void ConfigureRunner(DialogueRunner runner) {
        runner.AddScopedCommandHandler<string, string>("Call", CallCommand);
        runner.AddScopedCommandHandler<string>("CallBG", CallBGCommand);

    }

    Dictionary<string, object> GetVariablesFromRunner(DialogueRunner runner) {
        var varStore = (runner.VariableStorage as VariableStorageBehaviour);
        var variables = new Dictionary<string, object>();
        var (floats, strings, bools) = varStore.GetAllVariables();
        foreach (var kv in floats) {
            variables[kv.Key] = kv.Value;
        }
        foreach (var kv in strings) {
            variables[kv.Key] = kv.Value;
        }
        foreach (var kv in bools) {
            variables[kv.Key] = kv.Value;
        }
        return variables;
    }

    /// <yarncommand>Call</yarncommand>
    Coroutine CallCommand(DialogueRunner runner, string scriptName, string ownerName = null) {
        GameObject owner = null;
        if (ownerName != null) owner = GameObject.Find(ownerName);
        if (owner == null) owner = runner.gameObject;
        var ownerRunner = owner.GetComponent<MonoBehaviour>();
        Debug.Assert(ownerRunner != null, owner);

        bool done = false;
        Cutscenes.Instance.StartScript(
            scriptName,
            onDone: () => {done = true;},
            context: GetVariablesFromRunner(runner)
        );

        return ownerRunner.StartCoroutine(new WaitUntil(() => done));
    }

    /// <yarncommand>CallBG</yarncommand>
    void CallBGCommand(DialogueRunner runner, string scriptName) {
        Cutscenes.Instance.StartScript(
            scriptName,
            context: GetVariablesFromRunner(runner)
        );
    }
}