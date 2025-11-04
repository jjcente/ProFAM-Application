using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance;

    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource[] allAudioSources;

    void Awake()
    {
        // Singleton pattern to persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved preferences
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Update volume whenever a new scene loads
        SceneManager.sceneLoaded += (scene, mode) => ApplyVolumeToAll();
    }

    void Start()
    {
        ApplyVolumeToAll();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        ApplyVolumeToAll();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        ApplyVolumeToAll();
    }

    public void ApplyVolumeToAll()
    {
        allAudioSources = FindObjectsOfType<AudioSource>(true);

        foreach (AudioSource source in allAudioSources)
        {
            if (source == null) continue;

            // Simple rule: long, looping sounds are BGM; short, one-shots are SFX
            if (source.loop && source.clip != null && source.clip.length > 5f)
            {
                source.volume = musicVolume;
            }
            else
            {
                source.volume = sfxVolume;
            }
        }
    }
}
