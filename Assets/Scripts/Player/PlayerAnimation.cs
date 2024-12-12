using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Player player;
    public Animator animator;
    private PlayerMovement _playerMovement;

    private const string MOVEMENT = "m_Speed";
    private const string JUMP = "isJumping";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if (InputManager.instance.isMovementPressed)
        {
            if (_playerMovement.IsSprinting)
            {
                animator.SetFloat(MOVEMENT, 1f);
                player.playerStatistics.ReduceEndurancePeriodically(10.0f, delta);
            }
            else
            {
                animator.SetFloat(MOVEMENT, 0.5f);
            }
        }
        else
        {
            animator.SetFloat(MOVEMENT, 0);
        }

        if (!_playerMovement.IsGrounded())
        {
            animator.SetBool(JUMP, true);
            player.playerStatistics.ReduceEndurancePeriodically(10.0f, 1.0f);
        }
        else
        {
            animator.SetBool(JUMP, false);
        }
    }
}
