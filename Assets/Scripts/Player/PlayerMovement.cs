using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
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
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
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
            float angle = Mathf.SmoothDamp(transform.eulerAngles.y, targetAngle,
                ref turnSmoothVel,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _characterController.Move(moveDir * speed * Time.deltaTime);
        }
    }

    void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            _characterController.Move(new Vector3(0, -0.05f, 0));
        }
        else
        {
            _characterController.Move(new Vector3(0, gravity, 0));
        }
    }
}
