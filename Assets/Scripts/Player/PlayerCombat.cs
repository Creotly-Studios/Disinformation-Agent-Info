using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.attackPressed)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("punch punch punch");
    }
}
