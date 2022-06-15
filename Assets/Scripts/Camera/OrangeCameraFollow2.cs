using UnityEngine;
using System;


public class OrangeCameraFollow2 : MonoBehaviour {
    [Flags]
    public enum Direction {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = 3
    }


    public Transform target;
    public float dampTime = 0.15f;
    public Direction followType = Direction.Horizontal;
    [Range(0.0f, 1.0f)]
    public float
        cameraCenterX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float
        cameraCenterY = 0.5f;
    [Range(0.0f, 1.0f)]
    public float
        cameraCenterYForWarp = 0.5f;
    public Direction boundType = Direction.None;
    public float leftBound = 0;
    public float rightBound = 0;
    public float upperBound = 0;
    public float lowerBound = 0;
    public Direction deadZoneType = Direction.None;
    public bool hardDeadZone = false;
    public float leftDeadBound = 0;
    public float rightDeadBound = 0;
    public float upperDeadBound = 0;
    public float lowerDeadBound = 0;

    // private
    new Camera camera;
    Vector3 velocity = Vector3.zero;
    float vertExtent;
    float horzExtent;
    Vector3 tempVec = Vector3.one;
    bool isBoundHorizontal;
    bool isBoundVertical;
    bool isFollowHorizontal;
    bool isFollowVertical;
    bool isDeadZoneHorizontal;
    bool isDeadZoneVertical;
    Vector3 deltaCenterVec;

    void Start() {
        Initialize();
    }
    void OnValidate() {
        Initialize();
    }

    void Initialize() {
        camera = GetComponent<Camera>();
        vertExtent = camera.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;
        deltaCenterVec = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0))
            - camera.ViewportToWorldPoint(new Vector3(cameraCenterX, cameraCenterY, 0));

        isFollowHorizontal = (followType & Direction.Horizontal) == Direction.Horizontal;
        isFollowVertical = (followType & Direction.Vertical) == Direction.Vertical;
        isBoundHorizontal = (boundType & Direction.Horizontal) == Direction.Horizontal;
        isBoundVertical = (boundType & Direction.Vertical) == Direction.Vertical;

        isDeadZoneHorizontal = ((deadZoneType & Direction.Horizontal) == Direction.Horizontal) && isFollowHorizontal;
        isDeadZoneVertical = ((deadZoneType & Direction.Vertical) == Direction.Vertical) && isFollowVertical;
        tempVec = Vector3.one;
    }

    public void ClampTo(Grid grid) {
        var bounds = grid.GetTilemapsBounds();
        leftBound = bounds.min.x;
        rightBound = bounds.max.x;
        lowerBound = bounds.min.y;
        upperBound = bounds.max.y;
        OnValidate();
    }


    public void DoUpdateNoDeadZone() {
        var oldLeft = leftDeadBound;
        var oldRight = rightDeadBound;
        var oldUpper = upperDeadBound;
        var oldLower = lowerDeadBound;
        leftDeadBound = 0;
        rightDeadBound = 0;
        upperDeadBound = 0;
        lowerDeadBound = 0;
        var oldCameraCenterY = cameraCenterY;
        cameraCenterY = cameraCenterYForWarp;
        DoUpdateCamera();
        leftDeadBound = oldLeft;
        rightDeadBound = oldRight;
        upperDeadBound = oldUpper;
        lowerDeadBound = oldLower;
        cameraCenterY = oldCameraCenterY;
        needsUpdate = false;
    }

    bool needsUpdate = true;

    void Update() {
        needsUpdate = true;
    }

    void LateUpdate() {
        DoUpdateCamera();
    }

    void DoUpdateCamera() {
        if (target == null || !needsUpdate) return;
        var ctwp = camera.ViewportToWorldPoint(new Vector3(cameraCenterX, cameraCenterY, 0));
        Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(cameraCenterX, cameraCenterY, 0));

        if (!isFollowHorizontal) {
            delta.x = 0;
        }
        if (!isFollowVertical) {
            delta.y = 0;
        }

        if (!hardDeadZone) {
            Vector3 destination = transform.position + delta;
            tempVec = Vector3.SmoothDamp(camera.transform.position, destination, ref velocity, dampTime);
        } else {
            tempVec.Set(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
        }

        // Vector3 before = tempVec;
        if (isDeadZoneHorizontal) {
            if (delta.x > rightDeadBound) {
                tempVec.x = target.position.x - rightDeadBound + deltaCenterVec.x;
            }
            if (delta.x < -leftDeadBound) {
                tempVec.x = target.position.x + leftDeadBound + deltaCenterVec.x;
            }
        }
        if (isDeadZoneVertical) {
            if (delta.y > upperDeadBound) {
                tempVec.y = target.position.y - upperDeadBound + deltaCenterVec.y;
            }
            if (delta.y < -lowerDeadBound) {
                tempVec.y = target.position.y + lowerDeadBound + deltaCenterVec.y;
            }
        }
        // if (tempVec != before) {
        // 	Debug.Log($"Movement! {before} => {tempVec}  (ctwp={ctwp}; delta={delta})", this);
        // }

        if (isBoundHorizontal) {
            tempVec.x = Mathf.Clamp(tempVec.x, leftBound + horzExtent, rightBound - horzExtent);
        }

        if (isBoundVertical) {
            tempVec.y = Mathf.Clamp(tempVec.y, lowerBound + vertExtent, upperBound - vertExtent);
        }

        tempVec.z = camera.transform.position.z;
        camera.transform.position = tempVec;
    }
}

