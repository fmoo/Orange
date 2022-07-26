using UnityEngine;
using Yarn.Unity;
using System;

[CreateAssetMenu(fileName = "VariableUpdateChannel.asset", menuName = "Data/Channels/Yarn Variable Update Channel")]

public class VariableUpdateChannel : Channel<(string, object)> {
    public Func<string, object> GetValue;
}