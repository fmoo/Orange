using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeAudio : MonoBehaviour {
    [SerializeField] AudioSource audioSource;
    [SerializeField] OrangeAudioBank audioBank;
#if UNITY_EDITOR
    [NaughtyAttributes.Dropdown("GetAudioDropdown")]
#endif
    [SerializeField] private string soundName;

    public void Play() {
        audioBank.PlayEffect(audioSource, soundName);
    }

#if UNITY_EDITOR
    public NaughtyAttributes.DropdownList<string> GetAudioDropdown() {
        if (audioBank == null) return new NaughtyAttributes.DropdownList<string>();
        return audioBank.GetDropdown();
    }
#endif

    void OnValidate() {
        var button = GetComponent<UnityEngine.UI.Button>();
        if (button != null) {
            button.onClick.RemoveListener(Play);
            button.onClick.AddListener(Play);
        }
    }

    void Awake() {
        OnValidate();
    }

    void Start() {
        OnValidate();
    }
}
