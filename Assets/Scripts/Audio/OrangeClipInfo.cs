using UnityEngine;


[System.Serializable]
public class OrangeClipInfo {
    [Range(0f, 1f)]
    public float volumeMultiplier = 1.0f;
    // If clip is set, 
    public AudioClip clip;
    public bool loopEntireTrack = false;
    public float loopStart = 0f;
    public float loopDuration = 0f;

    public AudioClip introClip;
    public AudioClip loopClip;

    public bool DoesLoop {
        get {
            return (clip != null && loopDuration != 0f)
                || (introClip != null && loopClip != null)
                || (loopEntireTrack);
        }
    }
}