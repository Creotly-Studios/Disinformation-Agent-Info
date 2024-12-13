using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Player player;

    private bool hasHashed;
    private int movementHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        if (hasHashed)
        {
            return;
        }

        AnimatorHashing.StringToHash();
    }

    private void Start()
    {
        movementHash = Animator.StringToHash("m_Speed");
    }

    private void OnDisable()
    {
        if (hasHashed)
        {
            hasHashed = false;
        }
    }

    /// <summary>
    /// Handles Character Movement Animation
    /// </summary>
    public void SetBlendTreeParameter_Movement(float movementInput, bool isSprinting, float delta)
    {
        float snappedMovement = movementInput;

        if(isSprinting)
        {
            snappedMovement = 1.0f;
        }
        player.Animator.SetFloat(movementHash, snappedMovement, 0.1f, delta);
    }

    /// <summary>
    /// Handles Playing Animation with Root Motion
    /// </summary>
    public void PlayTargetAnimation(int targetAnimation, bool performAction, float transitionDuration = 0.1f)
    {
        player.Animator.applyRootMotion = performAction;
        player.Animator.SetBool(AnimatorHashing.performingActionHash, performAction);
        player.Animator.CrossFade(targetAnimation, transitionDuration);
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = (player.PlayerMovement.IsGrounded() != true);
        player.Animator.SetBool(AnimatorHashing.jumpingAnimatorHash, isGrounded);
    }
}
