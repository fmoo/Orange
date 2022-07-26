using Yarn.Unity;
using System.Collections;

public static class DialogueExtensions {
    // Helpers for command handlers that take DialogueRunner as the first callback
    public static void AddScopedCommandHandler(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner> handler) {
        runner.AddCommandHandler(commandHandler, () => handler(runner));
    }
    public static void AddScopedCommandHandler<T1>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1) => handler(runner, arg1));
    }
    public static void AddScopedCommandHandler<T1, T2>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1, T2> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2) => handler(runner, arg1, arg2));
    }
    public static void AddScopedCommandHandler<T1, T2, T3>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1, T2, T3> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3) => handler(runner, arg1, arg2, arg3));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1, T2, T3, T4> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4) => handler(runner, arg1, arg2, arg3, arg4));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4, T5>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1, T2, T3, T4, T5> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => handler(runner, arg1, arg2, arg3, arg4, arg5));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4, T5, T6>(this DialogueRunner runner, string commandHandler, System.Action<DialogueRunner, T1, T2, T3, T4, T5, T6> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => handler(runner, arg1, arg2, arg3, arg4, arg5, arg6));
    }
    public static void AddScopedCommandHandler(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, () => handler(runner));
    }
    public static void AddScopedCommandHandler<T1>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1) => handler(runner, arg1));
    }
    public static void AddScopedCommandHandler<T1, T2>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, T2, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2) => handler(runner, arg1, arg2));
    }
    public static void AddScopedCommandHandler<T1, T2, T3>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, T2, T3, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3) => handler(runner, arg1, arg2, arg3));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, T2, T3, T4, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4) => handler(runner, arg1, arg2, arg3, arg4));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4, T5>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, T2, T3, T4, T5, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => handler(runner, arg1, arg2, arg3, arg4, arg5));
    }
    public static void AddScopedCommandHandler<T1, T2, T3, T4, T5, T6>(this DialogueRunner runner, string commandHandler, System.Func<DialogueRunner, T1, T2, T3, T4, T5, T6, Coroutine> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => handler(runner, arg1, arg2, arg3, arg4, arg5, arg6));
    }

    public static void AddCommandHandler(this DialogueRunner runner, string commandHandler, System.Func<IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, () => runner.StartCoroutine(handler()));
    }
    public static void AddCommandHandler<T1>(this DialogueRunner runner, string commandHandler, System.Func<T1, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1) => runner.StartCoroutine(handler(arg1)));
    }
    public static void AddCommandHandler<T1, T2>(this DialogueRunner runner, string commandHandler, System.Func<T1, T2, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2) => runner.StartCoroutine(handler(arg1, arg2)));
    }
    public static void AddCommandHandler<T1, T2, T3>(this DialogueRunner runner, string commandHandler, System.Func<T1, T2, T3, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3) => runner.StartCoroutine(handler(arg1, arg2, arg3)));
    }
    public static void AddCommandHandler<T1, T2, T3, T4>(this DialogueRunner runner, string commandHandler, System.Func<T1, T2, T3, T4, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4) => runner.StartCoroutine(handler(arg1, arg2, arg3, arg4)));
    }
    public static void AddCommandHandler<T1, T2, T3, T4, T5>(this DialogueRunner runner, string commandHandler, System.Func<T1, T2, T3, T4, T5, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => runner.StartCoroutine(handler(arg1, arg2, arg3, arg4, arg5)));
    }
    public static void AddCommandHandler<T1, T2, T3, T4, T5, T6>(this DialogueRunner runner, string commandHandler, System.Func<T1, T2, T3, T4, T5, T6, IEnumerator> handler) {
        runner.AddCommandHandler(commandHandler, (T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => runner.StartCoroutine(handler(arg1, arg2, arg3, arg4, arg5, arg6)));
    }


}