using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource sfxSource;

    public AudioSource musicSource; // drag the music AudioSource here
    public AudioClip backgroundMusic; // assign your music clip
    public float musicVolume = 0.5f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    private void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void Update()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Optional: method to stop music
    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void FadeOutMusic(float duration = 1f)
    {
        if (musicSource == null || !musicSource.isPlaying)
            return;

        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

private System.Collections.IEnumerator FadeOutMusicCoroutine(float duration)
{
    float startVolume = musicSource.volume;
    float time = 0f;

    while (time < duration)
    {
        time += Time.deltaTime;
        musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
        yield return null;
    }

    musicSource.Stop();
    musicSource.volume = startVolume; // reset so next play has normal volume
}
}
