using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public int totalUnlockedLevels;
    public int currentSelectedIndex;

    public RectTransform[] levelSelectButtons;

    public Button selectLeft;
    public Button selectRight;
    public Button playButton;
    public Button menuButton;

    [SerializeField] private int startLevelIndex;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectLeft.onClick.AddListener(() =>
        {
            currentSelectedIndex--;
            if (currentSelectedIndex < 0)
            {
                currentSelectedIndex = totalUnlockedLevels - 1;
            }
            if (currentSelectedIndex > totalUnlockedLevels - 1)
            {
                currentSelectedIndex = 0;
            }
            SFXPlayer.Instance.PlayClickSound();
        });
        selectRight.onClick.AddListener(() =>
        {
            currentSelectedIndex++;
            if (currentSelectedIndex < 0)
            {
                currentSelectedIndex = totalUnlockedLevels - 1;
            }
            if (currentSelectedIndex > totalUnlockedLevels - 1)
            {
                currentSelectedIndex = 0;
            }
            SFXPlayer.Instance.PlayClickSound();
        });
        menuButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(0);
            SFXPlayer.Instance.PlayClickSound();
        });
        playButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(currentSelectedIndex + startLevelIndex);
            SFXPlayer.Instance.PlayClickSound();
        });
    }
    
    void Update()
    {
        for (int i = 0; i < levelSelectButtons.Length; i++)
        {
            if (currentSelectedIndex == i)
            {
                levelSelectButtons[i].localScale = Vector3.one * 1.2f;
                levelSelectButtons[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                levelSelectButtons[i].localScale = Vector3.one;
                levelSelectButtons[i].GetComponent<Button>().interactable = false;
            }
        }
    }
}
