using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions {

    public static IEnumerator WaitForAudioFade(this AudioSource audioSource, float targetVolume, float duration) {
        if (audioSource == null) yield break;
        float timeElapsed = 0f;
        float startVolume = audioSource.volume;

        while (timeElapsed < duration) {
            yield return null;
            timeElapsed += Time.deltaTime;
            var nextVolume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);
            audioSource.volume = nextVolume;
        }
        audioSource.volume = targetVolume;
    }

}
