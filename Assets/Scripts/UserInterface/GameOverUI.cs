using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
        [SerializeField] private Button menuBtn;
        [SerializeField] private Button replayBtn;
    
        // Start is called before the first frame update
        void Start()
        {
            GameManager.instance.OnStateChange += GameManager_OnGameStateChanged;
            menuBtn.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(0);
            });
            replayBtn.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            });
            Hide();
        }
    
        void GameManager_OnGameStateChanged(object sender, System.EventArgs e)
        {
            if (GameManager.instance.IsGameOver())
            {
                Show();
            }
            else Hide();
        }
    
        private void Show()
        {
            gameObject.SetActive(true);
        }
    
        private void Hide()
        {
            gameObject.SetActive(false);
        }
}
