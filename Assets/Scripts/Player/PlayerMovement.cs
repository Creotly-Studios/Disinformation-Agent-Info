using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Player _player;
    private PlayerData _playerData;
    
    private CharacterController _characterController;
    
    private Camera _camera;
    private Vector2 _input;
    private Vector3 _currentMoveInput;
    
    //
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GetComponent<Player>();
        _playerData = _player.playerData;
        
        SetCanMove(true);
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _sprintTimeRemaining = _playerData.sprintDuration;
        _sprintCooldownRemaining = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGravity();
        _input = InputManager.instance.currentMovementInput;
 
        Move();
        
    }

    public void Move()
    {
        Vector3 dir = new Vector3(_input.x, 0, _input.y);
        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, _playerData.turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move((moveDir * currentSpeed + _jumpForwardVelocity) * Time.deltaTime);
        }
    }

    void HandleGravity()
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
                _verticalVelocity = Mathf.Sqrt(_playerData.jumpHeight * -2f * _playerData.gravity);
                _jumpForwardVelocity = transform.forward * _playerData.jumpForwardForce;
            }
        }
        else
        {
            _verticalVelocity += _playerData.gravity * Time.deltaTime;
        }
    }

    float GetSpeedValue()
    {
        float givenSpeed;
        if (InputManager.instance.sprintPressed)
        {
            givenSpeed = _playerData.sprintSpeed;
            IsSprinting = true;
        }
        else
        {
            givenSpeed = _playerData.speed;
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
