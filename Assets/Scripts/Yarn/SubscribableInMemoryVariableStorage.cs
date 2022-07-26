using Yarn.Unity;
using UniRx;

public class SubscribableInMemoryVariableStorage : InMemoryVariableStorage {
    public readonly Subject<(string, object)> OnVariableChanged = new Subject<(string, object)>();
    public VariableUpdateChannel channel;

    void Init() {
        if (channel != null && channel.GetValue == null) {
            channel.GetValue = (k) => {
                if (TryGetValue<object>(k, out var result)) {
                    return result;
                }
                return null;
            };
        }
    }
    void Awake() {
        Init();
    }
    void Start() {
        Init();
    }

    public override void SetValue(string variableName, string stringValue) {
        base.SetValue(variableName, stringValue);
        if (channel != null) channel.emit((variableName, stringValue));
        OnVariableChanged.OnNext((variableName, stringValue));
    }

    public override void SetValue(string variableName, float floatValue) {
        base.SetValue(variableName, floatValue);
        if (channel != null) channel.emit((variableName, floatValue));
        OnVariableChanged.OnNext((variableName, floatValue));
    }

    public override void SetValue(string variableName, bool boolValue) {
        base.SetValue(variableName, boolValue);
        if (channel != null) channel.emit((variableName, boolValue));
        OnVariableChanged.OnNext((variableName, boolValue));
    }
}