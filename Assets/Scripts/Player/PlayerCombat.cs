using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    Player player;

    public List<PunchSO> combo;
    private float _lastClickedTime;
    private float _lastComboEnd;
    private int _comboCounter;

    public float timeBetweenCombos = 0.2f;
    public float timeBetweenAttackUsage = 0.2f;
    
    public float attackRange = 2f; // Range of the spherecast
    public float sphereRadius = 0.5f; // Radius of the sphere
    public Transform attackPoint;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.attackPressed && player.PlayerMovement.IsGrounded())
        {
            Attack();
        }
        ExitAttack();
    }

    private void Attack()
    {
        if (Time.time - _lastComboEnd > timeBetweenCombos && _comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");
            if (Time.time - _lastClickedTime >= timeBetweenAttackUsage)
            {
                player.Animator.runtimeAnimatorController = combo[_comboCounter].animatorOV;
                player.Animator.Play("Attack", 0, 0);
                player.PlayerMovement.SetCanMove(false);
                CheckAndDamage(combo[_comboCounter].damage);
                _comboCounter++;
                _lastClickedTime = Time.time;

                if (_comboCounter >= combo.Count)
                {
                    _comboCounter = 0;
                }
            }
        }
    }

    private void ExitAttack()
    {
        if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && player.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
            player.PlayerMovement.SetCanMove(true);
        }
    }

    void EndCombo()
    {
        _comboCounter = 0;
        _lastComboEnd = Time.time;
    }
    
    public void PlayerAttackScreenShake()
    {
       
        player.cameraImpulseSource.GenerateImpulse(0.75f);
        
      
    }

    void CheckAndDamage(int damage)
    {
            RaycastHit[] hits = Physics.SphereCastAll(attackPoint.position, sphereRadius, attackPoint.forward, attackRange);
            foreach (RaycastHit hit in hits)
            {
                // Check if the object hit has an enemy tag or component
                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    // Check if the enemy is in front of the player
                    Vector3 directionToEnemy = (hit.collider.transform.position - attackPoint.position).normalized;
                    float dotProduct = Vector3.Dot(attackPoint.forward, directionToEnemy);
    
                    if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                    {
                        Debug.Log($"Hit {hit.collider.name} in front!");
                        damagable.Damage(damage);
                    }
                }
            }
    }
    
    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, sphereRadius);
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + attackPoint.forward * attackRange);
            Gizmos.DrawWireSphere(attackPoint.position + attackPoint.forward * attackRange, sphereRadius);
        }
    }
    
}