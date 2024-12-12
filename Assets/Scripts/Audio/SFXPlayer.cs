using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance
    {
        get { return instance; }
    }
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SFX";
    private static SFXPlayer instance;

    public float sfxVolume = 1;
    private AudioSource _sfxSource;

    private void Awake()
    {
        sfxVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        _sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _sfxSource.volume = sfxVolume;
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void ChangeVolume()
    {
        sfxVolume += 0.1f;
        if (sfxVolume > 1f)
        {
            sfxVolume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return sfxVolume;
    }
    
    public void SetVolume(float _sfxVolume)
    {
        sfxVolume = 0;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _sfxVolume);
        PlayerPrefs.Save();
    }
}