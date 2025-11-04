using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (GlobalAudioManager.Instance != null)
        {
            musicSlider.value = GlobalAudioManager.Instance.musicVolume;
            sfxSlider.value = GlobalAudioManager.Instance.sfxVolume;

            musicSlider.onValueChanged.AddListener(GlobalAudioManager.Instance.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(GlobalAudioManager.Instance.SetSFXVolume);
        }
    }
}
