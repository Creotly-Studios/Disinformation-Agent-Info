using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    Player player;
    [SerializeField] private PlayerData playerData;
    private CharacterController _characterController;
    
    private Camera _camera;
    
    private Vector2 _input;
    private Vector3 _currentMoveInput;
    
    private float _turnSmoothVel;

    private float _verticalVelocity;
    private Vector3 _jumpForwardVelocity;

    public bool IsSprinting { get; private set; }
    
    public bool CanMove { get; private set; }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        SetCanMove(true);
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleGravityAndJump();
        _input = InputManager.instance.currentMovementInput;
        Move();
    }

    public void Move()
    {
        if (!CanMove)
            return;
        
        Vector3 dir = new Vector3(_input.x, 0, _input.y);
        float currentSpeed = GetSpeedValue();
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

    float GetSpeedValue()
    {
        float givenSpeed;
        if (InputManager.instance.sprintPressed && player.playerStatistics.CurrentEndurance > 0.15f)
        {
            givenSpeed = playerData.sprintSpeed;
            IsSprinting = true;
        }
        else
        {
            givenSpeed = playerData.speed;
            IsSprinting = false;
        }
        return givenSpeed;
    }

    public bool IsGrounded()
    {
        return _characterController.isGrounded;
    }

    public void SetCanMove(bool canMove)
    {
        CanMove = canMove;
    }
}
