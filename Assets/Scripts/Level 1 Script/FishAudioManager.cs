    using UnityEngine;
    using System.Collections;

    public class FishAudioManager : MonoBehaviour
    {
        public static FishAudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;   // main background music
        public AudioSource bubblesSource; // looping bubbles
        public AudioSource waterSource;   // looping water
        public AudioSource sfxSource;     // one-shot SFX

        [Header("Audio Clips")]
        public AudioClip backgroundMusic;
        public AudioClip bubbles;
        public AudioClip water;
        public AudioClip playerGrow;
        public AudioClip playerBite;
        public AudioClip stageClear;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float bubblesVolume = 1f;
        [Range(0f, 1f)] public float waterVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup background music
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.volume = musicVolume;
                musicSource.Play();
            }

            // Setup bubbles loop
            if (bubblesSource != null && bubbles != null)
            {
                bubblesSource.clip = bubbles;
                bubblesSource.loop = true;
                bubblesSource.volume = bubblesVolume;
                bubblesSource.Play();
            }

            // Setup water loop
            if (waterSource != null && water != null)
            {
                waterSource.clip = water;
                waterSource.loop = true;
                waterSource.volume = waterVolume;
                waterSource.Play();
            }
        }

        private void Update()
        {
            // Keep volumes updated in real-time
            if (musicSource != null) musicSource.volume = musicVolume;
            if (bubblesSource != null) bubblesSource.volume = bubblesVolume;
            if (waterSource != null) waterSource.volume = waterVolume;
        }

        // One-shot SFX
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public IEnumerator FadeOutAllBackground(float duration)
    {
        float elapsed = 0f;
        
        float startMusic = musicSource != null ? musicSource.volume : 0f;
        float startBubbles = bubblesSource != null ? bubblesSource.volume : 0f;
        float startWater = waterSource != null ? waterSource.volume : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (musicSource != null) musicSource.volume = Mathf.Lerp(startMusic, 0f, t);
            if (bubblesSource != null) bubblesSource.volume = Mathf.Lerp(startBubbles, 0f, t);
            if (waterSource != null) waterSource.volume = Mathf.Lerp(startWater, 0f, t);

            yield return null;
        }

        if (musicSource != null) musicSource.Stop();
        if (bubblesSource != null) bubblesSource.Stop();
        if (waterSource != null) waterSource.Stop();
    }


        public void PlayPlayerGrow() => PlaySFX(playerGrow);
        public void PlayPlayerBite() => PlaySFX(playerBite);
        public void PlayStageClear() => PlaySFX(stageClear);

        // Volume controls
        public void SetMusicVolume(float volume) => musicVolume = Mathf.Clamp01(volume);
        public void SetBubblesVolume(float volume) => bubblesVolume = Mathf.Clamp01(volume);
        public void SetWaterVolume(float volume) => waterVolume = Mathf.Clamp01(volume);

        public void RestartBackgroundAudio()
    {
        StopAllCoroutines();
        Debug.Log("🔊 Restarting background audio...");

        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        if (bubblesSource != null && bubbles != null)
        {
            bubblesSource.clip = bubbles;
            bubblesSource.loop = true;
            bubblesSource.volume = bubblesVolume;
            bubblesSource.Play();
        }

        if (waterSource != null && water != null)
        {
            waterSource.clip = water;
            waterSource.loop = true;
            waterSource.volume = waterVolume;
            waterSource.Play();
        }
    }

    }
