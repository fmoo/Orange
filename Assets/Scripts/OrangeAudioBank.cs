using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioBank", menuName = "Data/Audio Bank", order = 3)]
public class OrangeAudioBank : ScriptableObject {
    public OrangeAudioEntry[] clips = new OrangeAudioEntry[0];
    private Dictionary<string, OrangeAudioEntry> clipIndex = new Dictionary<string, OrangeAudioEntry>();

    private void Initialize() {
        if ((clipIndex?.Count ?? 0) > 0) return;
        foreach (var clip in clips) {
            clipIndex[clip.name] = clip;
        }
    }

    public void PlayLoopable(AudioSource source, AudioSource loopSource, string name) {
        var audio = GetAudio(name);
        if (!audio.loop) {
            Debug.LogError("Use PlayEffect for (non-looping) effects!");
            return;
        }
        source.clip = audio.clip;
        source.loop = audio.loop && audio.loopClip == null;
        source.Play();
        if (audio.loopClip != null) {
            if (audio.loopClip.loadState != AudioDataLoadState.Loaded) {
                Debug.LogWarning("The loop audio hasn't loaded yet!  You'll hear a gap on the first loop!  We should fix this.");
            }
            loopSource.playOnAwake = false;
            loopSource.loop = audio.loop;
            loopSource.volume = source.volume;
            loopSource.clip = audio.loopClip;
            loopSource.PlayDelayed(audio.clip.length);
        }
    }

    public void PlayEffect(AudioSource source, string name) {
        var audio = GetAudio(name);
        if (audio.loop) {
            Debug.LogError("Use PlayLoopable for looping audio!");
            return;
        }
        source.PlayOneShot(audio.clip, audio.volume);
    }

    public OrangeAudioEntry GetAudio(string clipName) {
        Initialize();
        if (clipIndex.TryGetValue(clipName, out OrangeAudioEntry audio)) {
            return audio;
        }
        Debug.LogError($"No audio for '{clipName} found in {name}");
        return null;
    }
    public AudioClip GetClip(string name) {
        Initialize();
        return GetAudio(name).clip;
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
public class OrangeAudioEntry {
    public string name;
    public AudioClip clip;
    public AudioClip loopClip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool loop = false;
}
