using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [Space]
    [SerializeField] private Button redirectButton;

    void Start()
    {
        menuButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(0);
        });
        redirectButton.onClick.AddListener(() =>
        {
            OpenDisinformationURL();
        });
    }

    void OpenDisinformationURL()
    {
        Application.OpenURL("https://gamejam-agentinfo.vercel.app");
    }
}
