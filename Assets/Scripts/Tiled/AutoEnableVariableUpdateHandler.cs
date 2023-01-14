using UniRx;
using UnityEngine;

public class AutoEnableVariableUpdateHandler : AutoVariableUpdateHandler {
    public override bool ShouldBeEnabled() {
        return (bool)channel.GetValue(variableName);
    }
}