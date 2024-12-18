using UnityEngine;
using UnityEngine.UI;

public class ComputerPanel_UI : MonoBehaviour
{
    private bool hasInitalized;

    [Header("User Buttons")]
    [SerializeField] private Button biasBingo_Btn;
    [SerializeField] private Button infoMatch_Btn;
    [SerializeField] private Button spotSource_Btn;

    [Header("Popup Panels")]
    [SerializeField] private NoticePopup popupPanel;

    [Header("User Interface")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject biasBingoPanel;
    [SerializeField] private GameObject infoMatchPanel;
    [SerializeField] public GameObject spotTheSourcePanel;

    private void OnEnable()
    {
        if(hasInitalized == true)
        {
            return;
        }

        hasInitalized = true;
        biasBingo_Btn.onClick.AddListener(() => DisplayPanel(biasBingoPanel));
        infoMatch_Btn.onClick.AddListener(() => DisplayPanel(infoMatchPanel));
        spotSource_Btn.onClick.AddListener(() => DisplayPanel(spotTheSourcePanel));
    }

    private void OnDisable()
    {
        if(hasInitalized != true)
        {
            return;
        }

        hasInitalized = false;
        biasBingo_Btn.onClick.RemoveListener(() => DisplayPanel(biasBingoPanel));
        infoMatch_Btn.onClick.RemoveListener(() => DisplayPanel(infoMatchPanel));
        spotSource_Btn.onClick.RemoveListener(() => DisplayPanel(spotTheSourcePanel));
    }

    public void DisablePanels()
    {
        mainMenuPanel.SetActive(true);

        biasBingoPanel.SetActive(false);
        infoMatchPanel.SetActive(false);
        spotTheSourcePanel.SetActive(false);
        popupPanel.gameObject.SetActive(false);
    }

    private void DisplayPanel(GameObject panel)
    {
        panel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void Start()
    {
        DisablePanels();
    }
}
