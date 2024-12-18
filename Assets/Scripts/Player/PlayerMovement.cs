using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private Camera _camera;
    private Vector2 _input;
    private Vector3 _currentMoveInput;

    private bool isSprinting;
    private float _turnSmoothVel;
    private float _verticalVelocity;
    private Vector3 _jumpForwardVelocity;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Internal SFX Settings")]
    [SerializeField] private float walkFootstepInterval = 0.5f; // Time between footsteps when walking
    [SerializeField] private float sprintFootstepInterval = 0.3f; // Time between footsteps when sprinting
    private float footstepTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        SetCanMove(true);
        _camera = Camera.main;
        InputManager.instance.InputSystemActions.Player.Jump.performed += _ => OnJumpStarted();
    }

    public void PlayerMovement_Update(float delta)
    {
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }

        HandleGravity(delta);
        _input = InputManager.instance.currentMovementInput;

        if (CanMove)
        {
            Move(delta);
        }
    }

    public void Move(float delta)
    {
        Vector3 dir = new Vector3(_input.x, 0, _input.y);
        isSprinting = (player.sprintFlag && InputManager.instance.isMovementPressed && player.PlayerStatistics.CurrentEndurance >= 10.5f);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, player.PlayerData.turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            float currentSpeed = GetSpeedValue();
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            player.CharacterController.Move((moveDir * currentSpeed + _jumpForwardVelocity) * delta);

             // Determine the footstep interval based on sprinting or walking
            float currentFootstepInterval = isSprinting ? sprintFootstepInterval : walkFootstepInterval;

            // Play footstep sound at intervals
            if (player.CharacterController.isGrounded && footstepTimer <= 0f)
            {
                PlayFootstepSound();
                footstepTimer = currentFootstepInterval;
            }
        }
        else
        {
            player.CharacterController.Move(_jumpForwardVelocity * delta);
        }

        if (isSprinting)
        {
            player.PlayerStatistics.ReduceEndurancePeriodically(10f, delta);
        }

        player.CharacterController.Move(new Vector3(0, _verticalVelocity, 0) * delta);

        float movemenentPressed = (InputManager.instance.isMovementPressed) ? 0.1f : 0f;
        player.PlayerAnimation.SetBlendTreeParameter_Movement(movemenentPressed, isSprinting, delta);

        if (footstepTimer > 0f)
        {
            footstepTimer -= delta;
        }
    }

    void HandleGravity(float delta)
    {
        if (player.CharacterController.isGrounded)
        {
            if (_verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
                _jumpForwardVelocity = Vector3.zero;
            }
        }
        else
        {
            _verticalVelocity += player.PlayerData.gravity * delta;
        }
    }

    void OnJumpStarted()
    {
        if (player.CharacterController.isGrounded && !DialogueManager.Instance.dialogueIsPlaying)
        {
            PlayJumpSound();
            _verticalVelocity = Mathf.Sqrt(player.PlayerData.jumpHeight * -2f * player.PlayerData.gravity);
            _jumpForwardVelocity = transform.forward * player.PlayerData.jumpForwardForce;
        }
    }

    float GetSpeedValue()
    {
        float givenSpeed = (isSprinting) ? player.PlayerData.sprintSpeed : player.PlayerData.speed;
        return givenSpeed;
    }

    public bool IsGrounded()
    {
        return player.CharacterController.isGrounded;
    }

    public void SetCanMove(bool canMove)
    {
        CanMove = canMove;
    }

    public bool CanMove { get; set; }

    public void PlayFootstepSound()
    {
        if (player.SFXPlayer.sfxList.playerFootStep.Length > 0)
        {

            int randomIndex = Random.Range(0, player.SFXPlayer.sfxList.playerFootStep.Length);
            AudioClip randomFootstep = player.SFXPlayer.sfxList.playerFootStep[randomIndex];

            // Play the selected sound
            SFXPlayer.Instance.PlaySFX(randomFootstep, player.SFXPlayer.GetVolume()/2f);
        }
    }


    public void PlayJumpSound()
    {
        SFXPlayer.Instance.PlaySFX(player.SFXPlayer.sfxList.playerJump);
    }
}
