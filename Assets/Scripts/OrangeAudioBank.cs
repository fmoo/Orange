using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioBank", menuName = "Data/Audio Bank", order = 3)]
public class OrangeAudioBank : ScriptableObject {
    public OrangeAudio[] clips = new OrangeAudio[0];
    private Dictionary<string, OrangeAudio> clipIndex = new Dictionary<string, OrangeAudio>();

    private void Initialize() {
        if ((clipIndex?.Count ?? 0) > 0) return;
        foreach (var clip in clips) {
            clipIndex[clip.name] = clip;
        }
    }

    public void PlaySound(AudioSource source, string name) {
        var sound = GetSound(name);
        source.PlayOneShot(sound.clip, sound.volume);
    }

    public OrangeAudio GetSound(string clipName) {
        Initialize();
        if (clipIndex.TryGetValue(clipName, out OrangeAudio audio)) {
            return audio;
        }
        Debug.LogError($"No audio for '{clipName} found in {name}");
        return null;
    }
    public AudioClip GetClip(string name) {
        Initialize();
        return GetSound(name).clip;
    }

    public NaughtyAttributes.DropdownList<string> GetDropdown() {
        var result = new NaughtyAttributes.DropdownList<string>();
        foreach (var clip in clips) {
            result.Add(clip.name, clip.name);
        }
        return result;
    }

    void Start() {
        Initialize();
    }
}

[System.Serializable]
public class OrangeAudio {
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}
