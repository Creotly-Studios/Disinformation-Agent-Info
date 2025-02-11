using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    Player_v2 player;
    PlayerData playerData;

    private float _lastClickedTime;
    private float _lastComboEnd;
    private int _comboCounter;

    private void Start()
    {
        player = GetComponent<Player_v2>();
        playerData = player.PlayerData;
    }

    void Update()
    {
        ExitAttack();
    }

    public void Attack()
    {
        if (Time.time - _lastComboEnd > playerData.timeBetweenCombos && _comboCounter <= playerData.attackArray.Count)
        {
            CancelInvoke("EndCombo");
            if (Time.time - _lastClickedTime >= playerData.timeBetweenAttackUsage)
            {
                player.MoveState.FreezeInput();
                playerData.attackArray[_comboCounter].PerformAttackAction(player.Anim);

                CheckAndDamage(playerData.attackArray[_comboCounter].damage);
                
                _comboCounter++;
                _lastClickedTime = Time.time;

                if (_comboCounter >= playerData.attackArray.Count)
                {
                    _comboCounter = 0;
                }
            }
        }
        player.MoveState.FreezeInput();
        
    }

    private void ExitAttack()
    {
        if (player.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && player.Anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    void EndCombo()
    {
        _comboCounter = 0;
        _lastComboEnd = Time.time;
    }

    void CheckAndDamage(int damage)
    {
        RaycastHit[] hits = Physics.SphereCastAll(player.checkTransform.position, playerData.attackSphereSize, player.checkTransform.forward, playerData.attackRange);
        foreach (RaycastHit hit in hits)
        {
            // Check if the object hit has an enemy tag or component
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hit.collider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    Debug.Log($"Hit {hit.collider.name} in front!");
                    damagable.TakeDamage(damage, AnimatorHashing.damageAnimation);
                }
            }
        }
    }

}
