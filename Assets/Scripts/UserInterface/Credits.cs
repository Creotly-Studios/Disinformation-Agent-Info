using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private Button menuButton;

    void Start()
    {
        menuButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(0);
        });
    }
}
