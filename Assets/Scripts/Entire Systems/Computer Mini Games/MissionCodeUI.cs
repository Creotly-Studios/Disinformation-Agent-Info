using TMPro;
using UnityEngine;

public class MissionCodeUI : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private TextMeshProUGUI missionCode;

    public void SetParameters(bool showCode)
    {
        QuestSO quest = QuestManager.Instance.ActiveQuest;
        if(quest == null)
        {
            return;
        }
        missionName.text = quest.name;
        missionCode.text = (showCode) ? $"Mission Code: {quest.QuestCode}" : "---";
    }
}
