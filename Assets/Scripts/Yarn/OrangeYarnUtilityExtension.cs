using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;

public class OrangeYarnUtilityExtension : OrangeYarnExtension {
    public Transform refsContainer;
    public GameObject refPrefab;

    public override void ConfigureRunner(DialogueRunner runner) {
        runner.AddFunction<int, int, int>("randomInt", FunctionRandomInt);
        runner.AddFunction<float, float, float>("random", FunctionRandom);
        runner.AddFunction<float>("time", FunctionTime);
        runner.AddFunction<float>("unscaledTime", FunctionUnscaledTime);
    }

    int FunctionRandomInt(int lowerBound, int upperBound) {
        return Random.Range(lowerBound, upperBound);
    }

    float FunctionRandom(float lowerBound, float upperBound) {
        return Random.Range(lowerBound, upperBound);
    }

    float FunctionTime() {
        return Time.timeSinceLevelLoad;
    }

    float FunctionUnscaledTime() {
        return Time.realtimeSinceStartup;
    }
}
