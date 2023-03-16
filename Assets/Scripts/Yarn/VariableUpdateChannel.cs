using UnityEngine;
using Yarn.Unity;
using System;

[CreateAssetMenu(fileName = "VariableUpdateChannel.asset", menuName = "Data/Channels/Yarn Variable Update Channel")]

public class VariableUpdateChannel : Channel<(string, object)> {

    public void Init(Func<string, object> getter) {
        this.getter = getter;
    }
    Func<string, object> getter;

    public object GetValue(string key) {
        if (getter == null) return null;
        return getter(key);
    }
}