using UniRx;
using UnityEngine;

public class AutoDisableVariableUpdateHandler : AutoVariableUpdateHandler {
    public override bool ShouldBeEnabled() {
        var result = channel.GetValue(variableName);
        return result != null ? !((bool)result) : true;
    }
}