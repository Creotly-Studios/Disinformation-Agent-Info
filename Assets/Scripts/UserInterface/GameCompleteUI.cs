using UnityEngine;
using UnityEngine.UI;

public class GameCompleteUI : MonoBehaviour
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button creditsButton;

        [SerializeField] private int menuSceneIndex;
        [SerializeField] private int creditsSceneIndex;


        void Start()
        {
            menuButton.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(menuSceneIndex);
            });
            creditsButton.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(creditsSceneIndex);
            });
        }
    }
