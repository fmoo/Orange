using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ScopedVariableStore : VariableStorageBehaviour {
    public string prefix;
    public VariableStorageBehaviour scopedData;
    public VariableStorageBehaviour parentData;

    public override void Clear() {
        scopedData.Clear();
    }

    public override void SetValue(string variableName, bool boolValue) {
        if (variableName.StartsWith(prefix)) {
            scopedData.SetValue(variableName, boolValue);
        } else {
            parentData.SetValue(variableName, boolValue);
        }
    }
    public override void SetValue(string variableName, float floatValue) {
        if (variableName.StartsWith(prefix)) {
            scopedData.SetValue(variableName, floatValue);
        } else {
            parentData.SetValue(variableName, floatValue);
        }
    }

    public override void SetValue(string variableName, string stringValue) {
        if (variableName.StartsWith(prefix)) {
            scopedData.SetValue(variableName, stringValue);
        } else {
            parentData.SetValue(variableName, stringValue);
        }
    }

    public override bool Contains(string variableName) {
        return scopedData.Contains(variableName) || parentData.Contains(variableName);
    }

    public override bool TryGetValue<T>(string variableName, out T result) {
        if (variableName.StartsWith(prefix)) {
            return scopedData.TryGetValue<T>(variableName, out result);
        } else {
            return parentData.TryGetValue<T>(variableName, out result);
        }
    }

    public override (Dictionary<string, float>, Dictionary<string, string>, Dictionary<string, bool>) GetAllVariables() {
        var floats = new Dictionary<string, float>();
        var strings = new Dictionary<string, string>();
        var bools = new Dictionary<string, bool>();
        return (floats, strings, bools);
    }

    public override void SetAllVariables(Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools, bool clear = true) {
        return;
    }
}
