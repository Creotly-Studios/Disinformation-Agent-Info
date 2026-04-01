using TMPro;
using UnityEngine;

public class MissionCodeUI : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private TextMeshProUGUI missionCode;

    public void SetParameters(bool showCode, QuestSO quest)
    {
        missionName.text = quest.name;
        missionCode.text = (showCode) ? $"Mission Code: {quest.QuestCode}" : "---";
    }
}
