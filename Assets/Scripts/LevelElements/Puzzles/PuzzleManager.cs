using System;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private Puzzle_Switches[] mainSwitches; // Must all be ON
    [SerializeField] private Puzzle_Switches[] blockerSwitches; // If any is ON, disable reward
    [SerializeField] private GameObject puzzleCompleteReward;

    private void Update()
    {
        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        bool allMainSwitchesOn = true; // Assume all main switches are ON

        // Check if all main switches are ON
        foreach (var switchObj in mainSwitches)
        {
            if (!switchObj.Switch) // If any main switch is OFF
            {
                allMainSwitchesOn = false;
                break;
            }
        }

        // Check if any blocker switch is ON
        bool anyBlockerOn = false;
        foreach (var switchObj in blockerSwitches)
        {
            if (switchObj.Switch) // If any blocker switch is ON
            {
                anyBlockerOn = true;
                break;
            }
        }

        // Reward is active only if all main switches are ON and no blocker switch is ON
        puzzleCompleteReward.SetActive(allMainSwitchesOn && !anyBlockerOn);
    }
}
