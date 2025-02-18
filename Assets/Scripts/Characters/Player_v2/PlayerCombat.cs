using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    private Coroutine attackRoutine;
    private Collider[] colliderArray;
    private RaycastHit[] rayCastHitArray;

    private int attackCount;
    private bool isAttacking;
    private Vector3 anchorPosition;

    private Enemy lockedTarget;
    private List<Enemy> tempColliderList = new();

    [Header("Parameter")]
    [SerializeField] private Transform debug1;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackCoolDown;
    [SerializeField] private PunchSO[] attackActions;
    [SerializeField] private LayerMask enemyLayerMask;

    private void Start()
    {
        colliderArray = new Collider[10];
        rayCastHitArray = new RaycastHit[15];
        for (int i = 0; i < attackActions.Length; i++) 
        {
            attackActions[i] = Instantiate(attackActions[i]);
            attackActions[i].Initialize();
        }

        anchorPosition = transform.position;
    }

    public void PlayerCombat_Updater(Player_v2 player)
    {
        //DetectAttackTarget(player);
        if(debug1 != null && lockedTarget != null) { debug1.position = lockedTarget.transform.position; }
    }

    public void HandleAttack(Player_v2 player)
    {
        if (player.InputHandler.AttackInput != true)
        {
            return;
        }
        HandleAttackCheck(player);
    }

    private void CheckAndDamage(int damage, Player_v2 player)
    {
        PlayerData playerData = player.PlayerData;
        Transform playerTransform = player.checkTransform;

        int count = Physics.SphereCastNonAlloc(playerTransform.position, playerData.attackRange, playerTransform.forward, rayCastHitArray);
        for(int i = 0; i < count; i++)
        {

            if (rayCastHitArray[i].collider == null)
            {
                continue;
            }

            Collider hitCollider = rayCastHitArray[i].collider;
            IDamagable damagable = hitCollider.GetComponent<IDamagable>();

            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hitCollider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    Debug.Log($"Hit {hitCollider.name} in front!");
                    damagable.TakeDamage(damage);
                }
            }
        }
    }

    //Would Change Target to Enemy Class
    private void HandleAttackCheck(Player_v2 player)
    {
        if(isAttacking)
        {
            return;
        }

        lockedTarget = DetectAttackTarget(player); 
        if(lockedTarget == null)
        {
            EnemyCombatControllerScript controller = EnemyCombatControllerScript.Instance;
            if(controller != null)
            {
                lockedTarget = controller.RandomGameObject(null);
            }
        }

        float moveDistance = Vector3.Distance(anchorPosition, player.transform.position);
        if(moveDistance > 1.0f)
        {
            anchorPosition = player.transform.position;
            lockedTarget = DetectAttackTarget(player);
        }

        if(lockedTarget == null)
        {
            EnemyCombatControllerScript controller = EnemyCombatControllerScript.Instance;
            if (controller != null)
            {
                lockedTarget = controller.RandomGameObject(null);
            }
        }

        float distance = (lockedTarget != null) ? Vector3.Distance(transform.position, lockedTarget.transform.position) : 0.0f;
        HandleAttack(distance, player);
    }

    private void HandleAttack(float distance, Player_v2 player)
    {
        int random = Random.Range(0, attackActions.Length);
        PunchSO noTargetAction = attackActions[random];

        if(lockedTarget == null)
        {
            noTargetAction.PerformAttackAction(player.Anim);
            CheckAndDamage(player.PlayerData.attackDamage, player);
            return;
        }

        if(distance < 15)
        {
            attackCount = (int)Mathf.Repeat((float)attackCount + 1, attackActions.Length);
            PunchSO currentAction = attackActions[attackCount];
            PerformAttackAction(player, currentAction, attackCoolDown, lockedTarget, 0.6f);
        }
        else
        {
            lockedTarget = null;
            noTargetAction.PerformAttackAction(player.Anim);
            CheckAndDamage(player.PlayerData.attackDamage, player);
        }
    }

    private void PerformAttackAction(Player_v2 player, PunchSO action, float coolDown, Enemy target, float duration)
    {
        action.PerformAttackAction(player.Anim);
        CheckAndDamage(player.PlayerData.attackDamage, player);

        if(attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }
        attackRoutine = StartCoroutine(AttackCoroutine(coolDown));

        if(target == null)
        {
            return;
        }
        MoveTowardsTarget(player, target.transform, duration);

        IEnumerator AttackCoroutine(float timer)
        {
            isAttacking = true;
            player.IdleState.FreezeInput();
            yield return new WaitForSeconds(timer);
            isAttacking = false;
        }
    }

    void MoveTowardsTarget(Player_v2 player, Transform target, float duration)
    {
        transform.DOLookAt(target.position, .2f);
        Vector3 targetOffset = TargetOffset(player.transform, target);
        StartCoroutine(MoveCharacter(player, targetOffset, duration));
    }

    private IEnumerator MoveCharacter(Player_v2 player, Vector3 targetPos, float duration)
    {
        float elapsed = 0.0f;
        Vector3 startPos = player.transform.position;

        while(elapsed  < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            player.controller.Move(newPos - player.transform.position);
            yield return null;
        }
    }

    public Vector3 TargetOffset(Transform player, Transform target)
    {
        Vector3 targetDir = (player.position - target.position).normalized;
        return target.position + (targetDir * 0.95f);
    }

    private Enemy DetectAttackTarget(Player_v2 player)
    {
        tempColliderList.Clear();
        int count = Physics.OverlapSphereNonAlloc(anchorPosition, attackRadius, colliderArray, enemyLayerMask);
        
        for(int i = 0; i < count; i++)
        {
            if (colliderArray[i] == null)
            {
                continue;
            }

            Enemy enemy = colliderArray[i].GetComponentInParent<Enemy>();
            if (tempColliderList.Contains(enemy))
            {
                continue;
            }
            tempColliderList.Add(enemy);
        }

        if(tempColliderList.Count > 0)
        {
            return RandomObject(lockedTarget);
        }
        return null;
    }

    private Enemy RandomObject(Enemy exclude)
    {
        List<int> objectIndex = new();
        for (int i = 0; i < tempColliderList.Count; i++)
        {
            if(tempColliderList[i].transform == exclude)
            {
                continue;
            }
            objectIndex.Add(i);
        }

        if(objectIndex.Count == 0)
        {
            return null;
        }
        int random = Random.Range(0, objectIndex.Count);

        int index = objectIndex[random];
        return tempColliderList[index];
    }
}