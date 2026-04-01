using UnityEngine;

[RequireComponent(typeof(Player_v2))]
public class PlayerCombatController : MonoBehaviour
{
    private Player_v2 player;

    [Header("Attack Data")]
    [SerializeField] private PunchSO[] attackArray;

    public bool IsInteractActive { get; private set; }
    private readonly Collider[] hitBuffer = new Collider[20];

    private void Awake()
    {
        player = GetComponent<Player_v2>();
        for(int i = 0; i < attackArray.Length; i++)
        {
            var attack = Instantiate(attackArray[i]);

            attack.Initialize();
            attackArray[i] = attack;
        }    
    }

    public void TryAttack()
    {
        if(player.performingAction)
        {
            return;
        }

        bool isMirror = Random.value > 0.55f;
        int index = Random.Range(0, attackArray.Length);
        PunchSO attack = attackArray[index];

        player.PlayAttackEffect();   
        attack.PerformAttackAction(isMirror, player.Animation);
        DealDamage(attack.damage);
    }

    // ── Called from PlayerGroundedState ───────────────────────────────────────

    public void TryInteract()
    {
        if(player.isAttacking)
        {
            return;
        }

        player.InputHandler.UseInteractInput();
        GameObject interactable = player.GetInteractableObject();
        if (interactable == null)
        {
            return;
        }
        AudioManager.Instance.PlaySFX(player.PlayerData.interact);
        interactable.GetComponent<IInteractable>().Interact(player);
        IsInteractActive = true;
    }

    // NonAlloc — no per-attack allocation.
    private void DealDamage(int damage)
    {
        bool hitSomething = false;
        int count = Physics.OverlapSphereNonAlloc(player.checkTransform.position, player.PlayerData.attackSphereSize, hitBuffer);
        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i] == null)
            {
                continue;
            }

            if (hitBuffer[i].TryGetComponent<IDamagable>(out var damagable))
            {
                Vector3 dir = (hitBuffer[i].transform.position - player.checkTransform.position).normalized;
                if (Vector3.Dot(player.checkTransform.forward, dir) <= 0.2f) continue;

                if (Physics.Linecast(player.checkTransform.position, hitBuffer[i].bounds.center, LayerMask.GetMask("Environment")))
                {
                    continue;
                }
                damagable.TakeDamage(damage);
                hitSomething = true;
            }
        }

        if (hitSomething)
        {
            AudioManager.Instance.PlaySFX(player.PlayerData.attackHit);
        }
    }

    public void SetInteractiveFlag(bool status)
    {
        IsInteractActive = status;
    }
}