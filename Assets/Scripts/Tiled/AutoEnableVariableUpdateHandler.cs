using UniRx;
using UnityEngine;

public class AutoEnableVariableUpdateHandler : AutoVariableUpdateHandler {
    public override bool ShouldBeEnabled() {
        if (channel == null) return false;
        var result = channel.GetValue(variableName);
        return result != null ? (bool)result : false;
    }
}