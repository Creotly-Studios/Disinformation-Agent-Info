using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button menuBtn;

    [Header("Primary Buttons")]
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button loadSaveBtn;

    [Header("Options")]
    [SerializeField] private Button optionsBtn;
    [SerializeField] private PauseMenuOptions optionsMenu;

    private void Start()
    {
        EventBus.Gameplay.OnGamePausedDisplay += OnGamePaused;
        menuBtn.onClick.AddListener(() => HandleLoadScene(0));
        replayBtn.onClick.AddListener(() => HandleLoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));

        resumeBtn.onClick.AddListener(() => GameManager.Instance.TogglePause());
        optionsBtn.onClick.AddListener(() => optionsMenu.gameObject.SetActive(true));
        loadSaveBtn.onClick.AddListener(() => EventBus.Save.OnDisplaySaveMenu?.Invoke());

        Hide();
    }

    private void OnDestroy()
    {
        EventBus.Gameplay.OnGamePausedDisplay -= OnGamePaused;
    }

    private void OnGamePaused(bool isPaused)
    {
        if (isPaused)
        {
            Show();
            return;
        }
        Hide();
    }

    private void HandleLoadScene(int index)
    {
        Hide();
        LevelLoader.LoadLevel(index);
        GameManager.Instance.ResetGame();
    }

    private void Show() => gameObject.SetActive(true);

    public void Hide()
    {
        gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
    }
}