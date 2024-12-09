using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private CharacterController _characterController;
    
    private Camera _camera;
    
    private Vector2 _input;
    private Vector3 _currentMoveInput;
    
    private float _turnSmoothVel;

    private float _verticalVelocity;
    private Vector3 _jumpForwardVelocity;

    public bool IsSprinting { get; private set; }
    private float _sprintTimeRemaining;
    private float _sprintCooldownRemaining;

    void Start()
    {
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _sprintTimeRemaining = playerData.sprintDuration;
        _sprintCooldownRemaining = 0f;
    }

    void Update()
    {
        HandleGravityAndJump();
        HandleSprintInput();
        _input = InputManager.instance.currentMovementInput;
        Move();
    }

    public void Move()
    {
        Vector3 dir = new Vector3(_input.x, 0, _input.y);
            float currentSpeed = IsSprinting ? playerData.sprintSpeed : playerData.speed;
        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, playerData.turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move((moveDir * currentSpeed + _jumpForwardVelocity) * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_jumpForwardVelocity * Time.deltaTime);
        }
        
        _characterController.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    void HandleGravityAndJump()
    {
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
                _jumpForwardVelocity = Vector3.zero;
            }
            
            if (InputManager.instance.jumpPressed)
            {
                _verticalVelocity = Mathf.Sqrt(playerData.jumpHeight * -2f * playerData.gravity);
                _jumpForwardVelocity = transform.forward * playerData.jumpForwardForce;
            }
        }
        else
        {
            _verticalVelocity += playerData.gravity * Time.deltaTime;
        }
    }

    void HandleSprintInput()
    {
        if (_sprintCooldownRemaining > 0) { _sprintCooldownRemaining -= Time.deltaTime; }

        if (IsSprinting)
        {
            _sprintTimeRemaining -= Time.deltaTime;

            if (_sprintTimeRemaining <= 0)
            {
                IsSprinting = false;
                _sprintCooldownRemaining = playerData.sprintCooldown;
            }
        }

        if (!IsSprinting && _sprintCooldownRemaining <= 0 && InputManager.instance.sprintPressed)
        {
            IsSprinting = true;
            _sprintTimeRemaining = playerData.sprintDuration;
        }
    }

    public bool IsGrounded()
    {
        return _characterController.isGrounded;
    }
}
