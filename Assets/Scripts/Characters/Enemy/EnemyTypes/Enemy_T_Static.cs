using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_T_Static : MonoBehaviour
{
    Enemy enemy;
    
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        //run logic for static enemies
        //could be that this static enemy rotates...
        //then we could turn it to an enemy that attacks what ever is in it's POV
        //possibilities endless ;)
    }
}
