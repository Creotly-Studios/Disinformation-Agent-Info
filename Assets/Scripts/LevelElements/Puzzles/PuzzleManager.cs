using System;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private Puzzle_Switches[] switches;
    [SerializeField] private GameObject puzzleCompleteReward;
    private bool allUnlocked = false;

    void Update()
    {
        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        allUnlocked = true; // Assume all switches are ON

        foreach (var switchObj in switches)
        {
            if (!switchObj.Switch) // If ANY switch is OFF
            {
                allUnlocked = false;
                break; // No need to check further
            }
        }

        puzzleCompleteReward.SetActive(allUnlocked);
    }
}
