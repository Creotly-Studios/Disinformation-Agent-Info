using UnityEngine;

public class PlayerMobileControlsUI : MonoBehaviour
{
    private bool isMobile;
    [SerializeField] private RectTransform controlsUI;

    private void Start()
    {
        isMobile =
        #if UNITY_ANDROID || UNITY_IOS
                true;
        #else
                false;
        #endif
        controlsUI.gameObject.SetActive(isMobile);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMobile)
        {
            return;
        }
        var player = Player_v2.Instance;
        var gameManager = GameManager.Instance;
        bool shouldDisableUI = ShouldDisableControlsUI(gameManager, player);
        controlsUI.gameObject.SetActive(!shouldDisableUI);
    }

    private bool ShouldDisableControlsUI(GameManager gameManager, Player_v2 player)
    {
        return gameManager.IsGameOver() ||
               gameManager.IsGamePaused() ||
               player.StateMachine.CurrentState == player.InactiveState;
    }
}
