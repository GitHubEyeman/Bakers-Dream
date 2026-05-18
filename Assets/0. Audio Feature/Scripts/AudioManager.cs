using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [TextArea(3, 10)] 
    public string designerNotes = ".";

    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Settings")]
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

    [Header("Pitch Randomization")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    [Header("Audio Clips")]
    [SerializeField] private List<Sound> musicTracks;
    [SerializeField] private List<Sound> soundEffects;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;

    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;

    private Coroutine fadeCoroutine;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildDictionaries();
        LoadVolumeSettings();

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;
    }

    private void BuildDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();

        foreach (Sound sound in musicTracks)
        {
            if (!musicDict.ContainsKey(sound.name))
                musicDict.Add(sound.name, sound.clip);
        }

        foreach (Sound sound in soundEffects)
        {
            if (!sfxDict.ContainsKey(sound.name))
                sfxDict.Add(sound.name, sound.clip);
        }
    }

    // ======================================================
    // MUSIC
    // ======================================================

    public void PlayMusic(string trackName, bool loop = true)
    {
        if (musicDict.TryGetValue(trackName, out AudioClip clip))
        {
            activeMusicSource.clip = clip;
            activeMusicSource.loop = loop;
            activeMusicSource.volume = musicVolume;
            activeMusicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music track not found: {trackName}");
        }
    }

    public void CrossfadeMusic(string trackName, float duration = 2f, bool loop = true)
    {
        if (!musicDict.TryGetValue(trackName, out AudioClip newClip))
        {
            Debug.LogWarning($"Music track not found: {trackName}");
            return;
        }

        // Prevent restarting same track
        if (activeMusicSource.clip == newClip)
            return;

        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.loop = loop;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(CrossfadeCoroutine(duration));
    }

    private IEnumerator CrossfadeCoroutine(float duration)
    {
        float timer = 0f;

        float startVolume = musicVolume;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            activeMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveMusicSource.volume = Mathf.Lerp(0f, startVolume, t);

            yield return null;
        }

        activeMusicSource.Stop();

        // Swap sources
        AudioSource temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;

        activeMusicSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        activeMusicSource.Stop();
        inactiveMusicSource.Stop();
    }

    // ======================================================
    // SOUND EFFECTS
    // ======================================================

    public void PlaySFX(string sfxName)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            // Randomized pitch
            sfxSource.pitch = Random.Range(minPitch, maxPitch);

            sfxSource.PlayOneShot(clip, sfxVolume);

            // Reset pitch
            sfxSource.pitch = 1f;
        }
        else
        {
            Debug.LogWarning($"SFX not found: {sfxName}");
        }
    }

    public void PlaySFX(string sfxName, float volumeMultiplier)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);

            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);

            sfxSource.pitch = 1f;
        }
        else
        {
            Debug.LogWarning($"SFX not found: {sfxName}");
        }
    }

    // ======================================================
    // VOLUME SETTINGS
    // ======================================================

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        activeMusicSource.volume = musicVolume;

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    // ======================================================
    // OPTIONAL UTILITIES
    // ======================================================

    public bool IsPlaying(string trackName)
    {
        return activeMusicSource.clip != null &&
               activeMusicSource.clip.name == trackName &&
               activeMusicSource.isPlaying;
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}