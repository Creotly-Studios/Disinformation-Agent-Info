using UnityEngine;
using UnityEngine.AI;

public class NPCFunctions : MonoBehaviour
{
    NPC npcManager;

    //Parameters
    private bool hasHashed;
    private Vector3 deltaPosition;

    //HashNames
    private int verticalMovementHash;
    private int horizontalMovementHash;

    private Vector3 verticalVelocity;

    [Header("Locomotion Parameters")]
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Gravity Parameters")]
    [SerializeField] protected float gravityForce = -30.0f;

    private void Awake()
    {
        npcManager = GetComponent<NPC>();
    }

    public void NPCFunctions_Update(float delta)
    {
        HandleGravity(delta);
    }

    private void OnEnable()
    {
        if (hasHashed)
        {
            return;
        }
        AnimatorHashing.StringToHash();
        verticalMovementHash = Animator.StringToHash("verticalMovement");
        horizontalMovementHash = Animator.StringToHash("horizontalMovement");
    }

    private void OnDisable()
    {
        if (hasHashed != true)
        {
            return;
        }
        hasHashed = false;
    }

    protected void OnAnimatorMove()
    {
        if (npcManager.isGrounded != true)
        {
            return;
        }

        deltaPosition = npcManager.animator.deltaPosition;
        npcManager.characterController.Move(deltaPosition);
        npcManager.transform.rotation *= npcManager.animator.deltaRotation;
    }

    //Functionalities

    #region Handle Movement

    private void HandleGravity(float delta)
    {
        npcManager.isGrounded = npcManager.characterController.isGrounded;

        if (npcManager.isGrounded)
        {
            if (verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }
        }
        else if (npcManager.isGrounded != true)
        {
            verticalVelocity.y += gravityForce * delta;
        }
        //Force of Gravity pushes character down
        npcManager.characterController.Move(verticalVelocity * delta);
    }

    public void PivotTowardsTarget()
    {
        if (npcManager.performingAction)
        {
            return;
        }

        if (npcManager.targetAngle >= 20 && npcManager.targetAngle <= 145)
        {
            PlayRootTargetAnimation(AnimatorHashing.turn_R_90, true);
        }
        else if (npcManager.targetAngle >= -145 && npcManager.targetAngle <= -20)
        {
            PlayRootTargetAnimation(AnimatorHashing.turn_L_90, true);
        }
        else if (npcManager.targetAngle > 145 && npcManager.targetAngle <= 180)
        {
            PlayRootTargetAnimation(AnimatorHashing.turn_R_180, true);
        }
        else if (npcManager.targetAngle < -145 && npcManager.targetAngle >= -180)
        {
            PlayRootTargetAnimation(AnimatorHashing.turn_L_180, true);
        }
    }

    public void RotateTowardsTarget()
    {
        if (npcManager.isMoving == true)
        {
            npcManager.transform.rotation = npcManager.navMeshAgent.transform.rotation;
        }
    }

    public void HandleMovement(Vector3 targetPosition, float speed)
    {
        if (npcManager.navMeshPath == null)
        {
            npcManager.navMeshPath = new NavMeshPath();
        }
        if (npcManager.navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            npcManager.navMeshPath.ClearCorners();
        }
        if (npcManager.navMeshAgent.CalculatePath(targetPosition, npcManager.navMeshPath))
        {
            npcManager.navMeshAgent.SetPath(npcManager.navMeshPath);
        }

        Vector3 moveDirection = npcManager.navMeshAgent.desiredVelocity;
        npcManager.characterController.Move(speed * Time.deltaTime * moveDirection);
    }

    #endregion

    #region Handle Animations

    public void SetBlendTreeParameter(float horizontalInput, float verticalInput, bool isSprinting, float delta)
    {
        float snappedVertical = verticalInput;
        float snappedHorizontal = horizontalInput;

        if (isSprinting)
        {
            snappedVertical = 2.0f;
            snappedHorizontal = 0.0f;
        }
        npcManager.animator.SetFloat(verticalMovementHash, snappedVertical, 0.1f, delta);
        npcManager.animator.SetFloat(horizontalMovementHash, snappedHorizontal, 0.1f, delta);
    }

    public void PlayRootTargetAnimation(int targetAnimation, bool performAction, float transitionDuration = 0.1f)
    {
        npcManager.animator.applyRootMotion = performAction;
        npcManager.animator.SetBool(AnimatorHashing.rootMotionRotateHash, true);
        npcManager.animator.SetBool(AnimatorHashing.isPerformingActionHash, performAction);
        npcManager.animator.CrossFade(targetAnimation, transitionDuration);
    }

    public void PlayTargetAnimation(int targetAnimation, bool performingAction, float transitionDuration = 0.2f, bool canRotate = true)
    {
        npcManager.animator.applyRootMotion = performingAction;
        npcManager.canRotate = canRotate;
        npcManager.animator.SetBool(AnimatorHashing.isPerformingActionHash, performingAction);
        npcManager.animator.CrossFade(targetAnimation, transitionDuration);
    }

    public void HandleAnimation(float maxDistance)
    {
        if (npcManager.targetDistance >= maxDistance)
        {
            SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
        }
    }

    #endregion
}
