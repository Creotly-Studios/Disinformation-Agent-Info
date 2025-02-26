using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip sceneMusic;
    
    void Start()
    {
        if (sceneMusic!=null)
        {
            AudioManager.Instance.PlayMusicWithXFade(sceneMusic);   
        }
    }
}
