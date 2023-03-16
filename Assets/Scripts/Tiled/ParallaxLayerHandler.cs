using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxLayerHandler : MonoBehaviour {
    public SuperLayer layer;
    public SpriteRenderer spriteRenderer;
    public Vector2 autoScrollSpeed = Vector2.zero;
    public Vector2 currentScrollOffset = Vector2.zero;
    public bool repeatsX = false;
    public bool repeatsY = false;

    private Vector3 v3 = new Vector3();

    public Vector3 origOffset;
    public Vector2 origSize;
    private Vector2 halfOrigSize;
    private Vector3 halfSize;

    void Start() {
        origOffset = transform.localPosition;
        origSize = spriteRenderer.size;
        halfOrigSize = origSize / 2f;
        if (repeatsX || repeatsY) {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.tileMode = SpriteTileMode.Continuous;

            // Check the source asset texture and log an error if it's not configured to repeat.
            if (spriteRenderer.sprite.texture.wrapMode != TextureWrapMode.Repeat) {
                Debug.LogError($"Sprite {spriteRenderer.sprite.texture.name} is not configured to repeat. Please set wrapMode to repeat.", spriteRenderer.sprite.texture);
            }

            // If repeatingX, double the horizontal size.  If repeatsY, double the vertical size.
            var size = origSize;
            var newPosition = transform.localPosition;
            if (repeatsX) {
                size.x *= 4f;
            }
            if (repeatsY) {
                size.y *= 4f;
            }
            spriteRenderer.size = size;
            halfSize = spriteRenderer.size / 2f;
        }
    }

    void UpdateScrollOffset() {
        // Automatically adjust currentScrollOffset based on autoScrollSpeed.  Snap to increments of size.
        if (autoScrollSpeed != Vector2.zero) {
            currentScrollOffset += autoScrollSpeed * Time.deltaTime;

            // NOTE: In theory we only need to the stuff below if we're repeating on these axes

            // If currentScrollOffset exceeds +/- the size of the sprite, snap to the nearest increment.
            while (currentScrollOffset.x > halfOrigSize.x) {
                currentScrollOffset.x -= origSize.x;
            }
            while (currentScrollOffset.x < -halfOrigSize.x) {
                currentScrollOffset.x += origSize.x;
            }
            // Same thing but for y
            while (currentScrollOffset.y > halfOrigSize.y) {
                currentScrollOffset.y -= origSize.y;
            }
            while (currentScrollOffset.y < -halfOrigSize.y) {
                currentScrollOffset.y += origSize.y;
            }
        }
    }

    void LateUpdate() {
        if (layer == null) return;
        var camera = Camera.main ?? Camera.current;
        if (camera == null) return;

        UpdateScrollOffset();

        v3.x = origOffset.x
            + (camera.transform.position.x * (1f - layer.m_ParallaxX))
            + (camera.rect.x / 2f * layer.m_ParallaxX * layer.m_OffsetX / 16f)
            + currentScrollOffset.x;
        v3.y = origOffset.y
            + (camera.transform.position.y * (1f - layer.m_ParallaxY))
            + (camera.rect.y / 2f * layer.m_ParallaxY * layer.m_OffsetY / 16f)
            + currentScrollOffset.y;
        v3.z = layer.transform.localPosition.z;

        layer.transform.position = v3;

        // Now, adjust so we fit within the camera viewport.
        // We want to adjust the layer's position so that it's center is within the viewport.
        if (repeatsX || repeatsY) {
            var safety = 0;

            // Compare the center of the sprite to the camera's position.
            var cameraCenter = camera.transform.position;
            var spriteCenter = layer.transform.position + halfSize;
            if (repeatsX) {
                // If the sprite is to the left of the camera, move it to the right.
                while (spriteCenter.x < cameraCenter.x - origSize.x) {
                    // Debug.LogWarning($"[{safety}] Adjusting layer {layer.name} to the right spriteCenter={v3} cameraCenter={cameraCenter} origSize={origSize}", this);
                    v3.x += origSize.x;
                    spriteCenter.x += origSize.x;
                    safety++;
                    if (safety > 30) {
                        Debug.LogError($"Infinite loop detected.  Please check your parallax layer configuration.");
                        break;
                    }
                }
                // If the sprite is to the right of the camera, move it to the left.
                while (spriteCenter.x > cameraCenter.x + origSize.x) {
                    // Debug.LogWarning($"[{safety}] Adjusting layer {layer.name} to the left spriteCenter={v3} cameraCenter={cameraCenter} origSize={origSize}", this);
                    v3.x -= origSize.x;
                    spriteCenter.x -= origSize.x;
                    safety++;
                    if (safety > 30) {
                        Debug.LogError($"Infinite loop detected.  Please check your parallax layer configuration.");
                        break;
                    }
                }
            }

            if (repeatsY) {
                // If the sprite is below the camera, move it up.
                while (spriteCenter.y < cameraCenter.y - origSize.y) {
                    // Debug.LogWarning($"[{safety}] Adjusting layer {layer.name} up spriteCenter={v3} cameraCenter={cameraCenter} origSize={origSize}", this);
                    v3.y += origSize.y;
                    spriteCenter.y += origSize.y;
                    safety++;
                    if (safety > 30) {
                        Debug.LogError($"Infinite loop detected.  Please check your parallax layer configuration.");
                        break;
                    }
                }
                // If the sprite is above the camera, move it down.
                while (spriteCenter.y > cameraCenter.y + origSize.y) {
                    // Debug.LogWarning($"[{safety}] Adjusting layer {layer.name} down spriteCenter={v3} cameraCenter={cameraCenter} origSize={origSize}", this);
                    v3.y -= origSize.y;
                    spriteCenter.y -= origSize.y;
                    safety++;
                    if (safety > 30) {
                        Debug.LogError($"Infinite loop detected.  Please check your parallax layer configuration.");
                        break;
                    }
                }
            }
        }

        layer.transform.position = v3;
    }
}