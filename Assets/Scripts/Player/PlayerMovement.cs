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

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCanMove(true);
        _camera = Camera.main;
    }

    // Update is called once per frame
    public void PlayerMovement_Update(float delta)
    {
        HandleGravity(delta);
        _input = InputManager.instance.currentMovementInput;
        Move(delta);
        
    }

    public void Move(float delta)
    {
        Vector3 dir = new Vector3(_input.x, 0, _input.y);
        isSprinting = (player.sprintFlag && InputManager.instance.isMovementPressed && player.PlayerStatistics.CurrentEndurance >= 10.5f);

        if (dir.magnitude >= 0.1f)
        {
            // Calculate the target angle based on input direction and camera's rotation
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
    
            // Smoothly transition to the target angle
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, player.PlayerData.turnSmoothTime);
    
            // Apply the rotation
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
    
            // Determine movement speed based on sprinting
            float currentSpeed = GetSpeedValue();

            // Move the character in the direction of the target rotation
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            player.CharacterController.Move((moveDir * currentSpeed + _jumpForwardVelocity) * delta);
        }
        else
        {
            // Apply only jump forward velocity if no movement input
            player.CharacterController.Move(_jumpForwardVelocity * delta);
        }

        // Apply vertical movement (gravity and jump) and Handle Player Movement Animations
        if(isSprinting)
        {
            player.PlayerStatistics.ReduceEndurancePeriodically(10f, delta);
        }
        player.CharacterController.Move(new Vector3(0, _verticalVelocity, 0) * delta);

        float movemenentPressed = (InputManager.instance.isMovementPressed) ? 0.1f : 0f;
        player.PlayerAnimation.SetBlendTreeParameter_Movement(movemenentPressed, isSprinting, delta);
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
            
            if (InputManager.instance.jumpPressed)
            {
                _verticalVelocity = Mathf.Sqrt(player.PlayerData.jumpHeight * -2f * player.PlayerData.gravity);
                _jumpForwardVelocity = transform.forward * player.PlayerData.jumpForwardForce;
            }
        }
        else
        {
            _verticalVelocity += player.PlayerData.gravity * delta;
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
}
