using UnityEngine;

public class SetAttackOverHelper : MonoBehaviour
{
    public Player_v2 player;


    public void AttackOver()
    {
        player.AtttackState.AnimationFinishTrigger();
    }
}
