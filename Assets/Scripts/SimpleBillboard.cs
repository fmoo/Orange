using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleBillboard : MonoBehaviour
{
    public enum BillboardMethod
    {
        LOOK_AT_CAMERA = 0,
        INVERT_CAMERA = 1,
    }

    public BillboardMethod billboardMethod = BillboardMethod.INVERT_CAMERA;

    private static Camera sharedCamera;

    void Update()
    {
        // TODO: memoize this per-frame across all instances of this script
        Camera camera = null;
        if (Application.isEditor && Application.isPlaying)
        {
            camera = Camera.main;
        }
        else
        {
            camera = Camera.current;
        }

        if (!camera)
        {
            return;
        }

        if (billboardMethod == BillboardMethod.LOOK_AT_CAMERA)
        {
            transform.LookAt(camera.transform.position, Vector3.up);
        }
        else if (billboardMethod == BillboardMethod.INVERT_CAMERA)
        {
            transform.rotation = camera.transform.rotation;
            transform.Rotate(Vector3.up, 180f);
        }
    }
}
