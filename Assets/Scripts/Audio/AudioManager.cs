using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get { return instance; } }
    private static AudioManager instance;

    public float musicVolume = 1;
    public float sfxVolume = 1;

    private AudioSource music1;
    private AudioSource music2;
    private AudioSource sfxSource;

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup soundMixer;
    private AudioMixer music;
    private AudioMixer sound;

    public AudioSource currentMusicAudioSource;

    public static bool musicOn = true, audioOn = true;
    private bool firstMusicSourceActive;

    [Header("DATA OBJECTS")]
    public SoundEffects soundEffects;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        music = musicMixer.audioMixer;
        sound = soundMixer.audioMixer;

        music1 = gameObject.AddComponent<AudioSource>();
        music2 = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        music1.loop = true;
        music2.loop = true;

        music1.outputAudioMixerGroup = musicMixer;
        music2.outputAudioMixerGroup = musicMixer;
        sfxSource.outputAudioMixerGroup = soundMixer;

        // Load volume settings
        musicVolume = PlayerPrefs.GetFloat("Music", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        audioOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        UpdateMixerVolumes();
    }

    private void UpdateMixerVolumes()
    {
        music.SetFloat("Volume", musicOn ? Mathf.Log10(musicVolume) * 20 : -80);
        sound.SetFloat("Volume", audioOn ? Mathf.Log10(sfxVolume) * 20 : -80);
        
        if (currentMusicAudioSource != null) { currentMusicAudioSource.volume = musicVolume; }
    }

    public void ChangeMusicVolume()
    {
        musicVolume += 0.1f;
        if (musicVolume > 1f)
        {
            musicVolume = 0f;
        }
        PlayerPrefs.SetFloat("Music", musicVolume);
        PlayerPrefs.Save();
        currentMusicAudioSource.volume = musicVolume;
        UpdateMixerVolumes();
    }

    public void ChangeSFXVolume()
    {
        sfxVolume += 0.1f;
        if (sfxVolume > 1f)
        {
            sfxVolume = 0f;
        }
        PlayerPrefs.SetFloat("SFX", sfxVolume);
        PlayerPrefs.Save();
        UpdateMixerVolumes();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayMusicWithXFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        StartCoroutine(CrossfadeMusic(newClip, transitionTime));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float transitionTime)
    {
        AudioSource activeSource = firstMusicSourceActive ? music1 : music2;
        AudioSource newSource = firstMusicSourceActive ? music2 : music1;

        newSource.clip = newClip;
        newSource.volume = 0;
        newSource.Play();

        float timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            activeSource.volume = Mathf.Lerp(musicVolume, 0, t);
            newSource.volume = Mathf.Lerp(0, musicVolume, t);
            yield return null;
        }

        activeSource.Stop();
        firstMusicSourceActive = !firstMusicSourceActive;
        currentMusicAudioSource = newSource;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }






}
