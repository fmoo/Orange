using UniRx;
using UnityEngine;

abstract public class AutoVariableUpdateHandler : MonoBehaviour {
    public VariableUpdateChannel channel;

    public string variableName;

    void Start() {
        channel.events.TakeUntilDestroy(this)
            .Subscribe(OnVariableUpdated);
        if (!ShouldBeEnabled()) {
            gameObject.SetActive(false);
        }
    }

    abstract public bool ShouldBeEnabled();

    void OnVariableUpdated((string Name, object Value) parts) {
        if (parts.Name != variableName) return;
        gameObject.SetActive(ShouldBeEnabled());
    }
}