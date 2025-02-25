using UnityEngine;

public class PlayerInteractState : PlayerAbilityState
{
    public PlayerInteractState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        if (GetInteractableObject() != null)
        {
            GetInteractableObject().GetComponent<IInteractable>().Interact(player);
        }
        player.InputHandler.UseInteractInput();
        // isAbilityDone = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Debug.Log(GetInteractableObject());
        
        AnimatorStateInfo stateInfo = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.95f && stateInfo.IsTag("interact"))
        {
            isAbilityDone = true; // Mark the ability as done
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public GameObject GetInteractableObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(player.checkTransform.position, playerData.detectRadius, player.checkTransform.forward, playerData.detectRange);
        foreach (RaycastHit hit in hits)
        {
            GameObject inter = hit.collider.gameObject;
            IInteractable interactable = inter.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Vector3 directionToEnemy = (hit.collider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    return inter;
                }
            }
        }
        return null;
    }

    public void SetAbilityDone()
    {
        isAbilityDone = true;
    }
}