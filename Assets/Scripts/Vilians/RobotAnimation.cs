using UnityEngine;

public class RobotAnimation : MonoBehaviour
{
    Robot robot;

    //Parameters
    private bool hasHashed;
    private Vector3 deltaPosition;

    //HashNames
    private int verticalMovementHash;
    private int horizontalMovementHash;

    protected virtual void Awake()
    {
        robot = GetComponent<Robot>();
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

    private void OnAnimatorMove()
    {
        if (robot.isGrounded != true)
        {
            return;
        }

        deltaPosition = robot.animator.deltaPosition;
        robot.characterController.Move(deltaPosition);
        robot.transform.rotation *= robot.animator.deltaRotation;
    }

    public virtual void CharacterAnimatorManager_Update(float delta)
    {

    }

    //Functionalities
    public void SetBlendTreeParameter(float horizontalInput, float verticalInput, bool isSprinting, float delta)
    {
        float snappedVertical = verticalInput;
        float snappedHorizontal = horizontalInput;

        if (isSprinting)
        {
            snappedVertical = 2.0f;
            snappedHorizontal = 0.0f;
        }
        robot.animator.SetFloat(verticalMovementHash, snappedVertical, 0.1f, delta);
        robot.animator.SetFloat(horizontalMovementHash, snappedHorizontal, 0.1f, delta);
    }

    public void PlayRootTargetAnimation(int targetAnimation, bool performAction, float transitionDuration = 0.1f)
    {
        robot.animator.applyRootMotion = performAction;
        robot.animator.SetBool(AnimatorHashing.rootMotionRotateHash, true);
        robot.animator.SetBool(AnimatorHashing.isPerformingActionHash, performAction);
        robot.animator.CrossFade(targetAnimation, transitionDuration);
    }

    public void PlayTargetAnimation(int targetAnimation, bool performingAction, float transitionDuration = 0.2f, bool canRotate = true)
    {
        robot.animator.applyRootMotion = performingAction;
        robot.canRotate = canRotate;
        robot.animator.SetBool(AnimatorHashing.isPerformingActionHash, performingAction);
        robot.animator.CrossFade(targetAnimation, transitionDuration);
    }

    private float HaltRunning(float maxDistance)
    {
        return maxDistance + 2.5f;
    }

    public void HandleAnimation(float maxDistance)
    {
        if (robot.DistanceToTarget >= maxDistance)
        {
            float shouldRun = HaltRunning(maxDistance);
            if (robot.DistanceToTarget > shouldRun)
            {
                SetBlendTreeParameter(0f, 2.0f, true, Time.deltaTime);
                return;
            }
            SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
        }
    }
}
