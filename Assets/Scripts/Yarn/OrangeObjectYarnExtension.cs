using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class OrangeObjectYarnExtension : OrangeYarnExtension {
    public Transform refsContainer;
    public GameObject refPrefab;

    public override void ConfigureRunner(DialogueRunner runner) {
        runner.AddCommandHandler<string>("Destroy", CommandDestroy);
        runner.AddCommandHandler<string>("PrefetchPrefab", CommandPrefetchPrefab);

        runner.AddFunction<string, string>("createRef", FunctionCreateRef);
        runner.AddFunction<string, string, string>("createPrefab", FunctionCreatePrefab);

        runner.AddFunction<string, string, bool, bool>("getBool", FunctionGetBool);
        runner.AddFunction<string, string, float, float>("getNum", FunctionGetNum);
        runner.AddFunction<string, string, string, string>("getStr", FunctionGetStr);
    }

    public void CommandDestroy(string objectId) {
        var srcObj = GameObject.Find(objectId);
        Destroy(srcObj);
    }

    public string FunctionCreateRef(string dest) {
        var destRef = GameObject.Find(dest);

        var refObj = Instantiate(refPrefab, refsContainer);
        refObj.name = System.Guid.NewGuid().ToString();
        refObj.transform.position = destRef.transform.position;
        return refObj.name;
    }

    public Coroutine CommandPrefetchPrefab(string prefabAddress) {
        var loading = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
        return StartCoroutine(loading);
    }


    public string FunctionCreatePrefab(string prefabAddress, string dest) {
        var destRef = GameObject.Find(dest);
        var addressablePrefab = Addressables.LoadAssetAsync<GameObject>(prefabAddress).WaitForCompletion();

        var prefabObj = Instantiate(addressablePrefab, refsContainer);
        prefabObj.name = System.Guid.NewGuid().ToString();

        prefabObj.transform.position = destRef.transform.position;
        return prefabObj.name;
    }

    public string FunctionGetStr(string objectId, string propName, string defaultValue = "") {
        if (TryFindComponent<OrangeSpriteAnimator>(objectId, out var animator)) {
            if (animator.sprites.strings.TryGetValue(propName, out var value)) {
                return value;
            }
        }
        return defaultValue;
    }
    public float FunctionGetNum(string objectId, string propName, float defaultValue = 0f) {
        if (TryFindComponent<OrangeSpriteAnimator>(objectId, out var animator)) {
            if (animator.sprites.variables.TryGetValue(propName, out var value)) {
                return value;
            }
        }
        return defaultValue;
    }
    public bool FunctionGetBool(string objectId, string propName, bool defaultValue = false) {
        if (TryFindComponent<OrangeSpriteAnimator>(objectId, out var animator)) {
            if (animator.sprites.flags.TryGetValue(propName, out var value)) {
                return value;
            }
        }
        return defaultValue;
    }
}
