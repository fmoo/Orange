using Yarn.Unity;
using System.Linq;
using UnityEngine;

public class OrangeYarnDebugExtension : OrangeYarnExtension {
    public override void ConfigureRunner(DialogueRunner runner) {
        runner.AddCommandHandler<string, string, string, string, string, string>("TODO", CommandTodo);
        runner.AddCommandHandler<string, string, string, string, string, string>("Log", CommandLog);
        runner.AddCommandHandler<string, string, string, string, string, string>("Warn", CommandWarn);
        runner.AddCommandHandler<string, string, string, string, string, string>("Error", CommandError);
    }

    public void CommandTodo(string arg1, string arg2 = "", string arg3 = "", string arg4 = "", string arg5 = "", string arg6 = "") {
        var args = new string[] { "TODO:", arg1, arg2, arg3, arg4, arg5, arg6 }.Where(s => !string.IsNullOrEmpty(s));
        Debug.Log(string.Join(" ", args), this);
    }

    public void CommandLog(string arg1, string arg2 = "", string arg3 = "", string arg4 = "", string arg5 = "", string arg6 = "") {
        var args = new string[] { arg1, arg2, arg3, arg4, arg5, arg6 }.Where(s => !string.IsNullOrEmpty(s));
        Debug.Log(string.Join(" ", args), this);
    }

    public void CommandWarn(string arg1, string arg2 = "", string arg3 = "", string arg4 = "", string arg5 = "", string arg6 = "") {
        var args = new string[] { arg1, arg2, arg3, arg4, arg5, arg6 }.Where(s => !string.IsNullOrEmpty(s));
        Debug.LogWarning(string.Join(" ", args), this);
    }

    public void CommandError(string arg1, string arg2 = "", string arg3 = "", string arg4 = "", string arg5 = "", string arg6 = "") {
        var args = new string[] { arg1, arg2, arg3, arg4, arg5, arg6 }.Where(s => !string.IsNullOrEmpty(s));
        Debug.LogError(string.Join(" ", args), this);
    }
}