using UnityEngine;

public class PlayerMobileControlsUI : MonoBehaviour
{
    [SerializeField] private RectTransform controlsUI;

    // Update is called once per frame
    void Update()
    {
        var gameManager = GameManager.Instance;
        var player = Player_v2.Instance;

        if (ShouldDisableControlsUI(gameManager, player))
        {
            controlsUI.gameObject.SetActive(false);
        }
        else
        {
            controlsUI.gameObject.SetActive(true);
        }
    }

    private bool ShouldDisableControlsUI(GameManager gameManager, Player_v2 player)
    {
        return gameManager.IsGameOver() ||
               gameManager.IsGamePaused() ||
               player.StateMachine.CurrentState == player.InactiveState;
    }
}
