using UniRx;
using UnityEngine;

public class AutoDisableVariableUpdateHandler : AutoVariableUpdateHandler {
    public override bool ShouldBeEnabled() {
        return !(bool)channel.GetValue(variableName);
    }
}