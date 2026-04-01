using UnityEngine;

[RequireComponent(typeof(Player_v2))]
public class PlayerLocomotionManager : MonoBehaviour
{
    private Player_v2 player;
    private Transform cameraObject;
    private CharacterController controller;

    [SerializeField] private float verticalVelocity;

    private float currentSprintTime;
    public float CurrentMoveSpeed { get; private set; }
    private float lastSprintExhaustedTime = -Mathf.Infinity;

    private float footstepTimer;
    public bool CanDash() => currentSprintTime >= player.PlayerData.dashStaminaCost;

    private void Awake()
    {
        player = GetComponent<Player_v2>();
        cameraObject = Camera.main.transform;
    }

    private void Start()
    {
        controller = player.Controller;
        CurrentMoveSpeed = player.PlayerData.speed;
        currentSprintTime = player.PlayerData.sprintDuration;
    }

    public void Locomotion_Update(float delta)
    {
        HandleGravity(delta);
        DispatchMovement(delta);
        HandleSprint(delta);
    }

    private void DispatchMovement(float delta)
    {
        if (player.isDead) return;

        AbilityState current = player.CurrentAbilityState;
        current.HandleRotation(delta, player);
        current.HandleMovement(delta, player);
    }

    private void HandleGravity(float delta)
    {
        if (player.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += player.PlayerData.gravity * delta;
            verticalVelocity = Mathf.Max(verticalVelocity, -53f);
        }
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * delta);
        player.isGrounded = controller.isGrounded;
    }

    public void ExecuteJump()
    {
        verticalVelocity = Mathf.Sqrt(player.PlayerData.jumpHeight * -2f * player.PlayerData.gravity);
        AudioManager.Instance.PlaySFX(player.PlayerData.jump);
    }

    public void SetVerticalVelocity(float velocity) => verticalVelocity = velocity;
    public float GetVerticalVelocity() => verticalVelocity;

    public void ExecuteDash()
    {
        currentSprintTime = Mathf.Max(0f, currentSprintTime - player.PlayerData.dashStaminaCost);
        AudioManager.Instance.PlaySFX(player.PlayerData.dash);
        player.PlayDashEffect();
    }

    // ── Sprint ────────────────────────────────────────────────────────────────

    private void HandleSprint(float delta)
    {
        PlayerData data = player.PlayerData;
        bool sprint = player.InputHandler.SprintInput;

        if (sprint && currentSprintTime > 0f
            && Time.time - lastSprintExhaustedTime > data.sprintCooldown)
        {
            CurrentMoveSpeed = data.sprintSpeed;
            currentSprintTime -= delta;
            player.Animation.SetSprintBlend(1f, delta);
        }
        else
        {
            CurrentMoveSpeed = data.speed;
            player.Animation.SetSprintBlend(0f, delta);
            if (currentSprintTime < data.sprintDuration)
                currentSprintTime += delta * data.sprintRechargeRate;
        }

        if (currentSprintTime <= 0f)
            lastSprintExhaustedTime = Time.time;

        if (player.SprintUIBar != null)
            player.SprintUIBar.fillAmount = currentSprintTime / data.sprintDuration;
    }

    // ── Footsteps ─────────────────────────────────────────────────────────────

    // Called from Normal_AbilityState.HandleMovement when the player is moving.
    public void TickFootsteps(float delta)
    {
        footstepTimer -= delta;
        if (footstepTimer > 0f) return;

        int index = Random.Range(0, player.PlayerData.footsteps.Length);
        AudioManager.Instance.PlaySFX(player.PlayerData.footsteps[index]);
        footstepTimer = player.PlayerData.footstepInterval;
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    // Converts raw 2D stick input into a camera-relative world direction.
    // Called by AbilityState SOs — single implementation, no duplication.
    public Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 camRight = Vector3.ProjectOnPlane(cameraObject.right, Vector3.up).normalized;
        Vector3 camForward = Vector3.ProjectOnPlane(cameraObject.forward, Vector3.up).normalized;
        return (camRight * input.x + camForward * input.y).normalized;
    }
}