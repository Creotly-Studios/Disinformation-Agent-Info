using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance
    {
        get { return instance; }
    }
    private const string PLAYER_PREFS_MUSIC_VOLUME = "Music";
    private static MusicManager instance;

    public float musicVolume = 1;
    private AudioSource _musicSource;
    [SerializeField] private GameObject musicManagerGameObject;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        musicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME);
        _musicSource = musicManagerGameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        _musicSource.volume = musicVolume;
    }

    public void ChangeVolume()
    {
        musicVolume += 0.1f;
        if (musicVolume > 1f)
        {
            musicVolume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return musicVolume;
    }
    
    public void SetVolume(float _mVolume)
    {
        musicVolume = 0;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, _mVolume);
        PlayerPrefs.Save();
    }
}
