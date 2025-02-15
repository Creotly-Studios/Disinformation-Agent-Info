using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    private Transform cameraObject;
    private RaycastHit[] rayCastHitArray;

    public Robot currentTarget { get; private set; }
    public float inputDirectionMagnitude { get; private set; }

    [Header("Parameter")]
    [SerializeField] private Transform debug1, debug2;
    [SerializeField] private LayerMask enemyLayerMask;

    private void Start()
    {
        rayCastHitArray = new RaycastHit[15];
        cameraObject = Camera.main.transform;
    }

    public void SetTarget(Robot robot)
    {
        currentTarget = robot;
        if (debug1 != null) { debug1.transform.position = robot.transform.position; }
    }

    public void PlayerCombat_Updater(Player_v2 player)
    {
        DetectAttackTarget(player);
    }

    private void DetectAttackTarget(Player_v2 player)
    {
        Vector3 inputDirection;
        PlayerInputHandler inputHandler = player.InputHandler;

        inputDirection = cameraObject.forward * inputHandler.CameraInput.y;
        inputDirection += cameraObject.right * inputHandler.CameraInput.x;

        inputDirection.y = 0.0f;
        inputDirection.Normalize();

        inputDirectionMagnitude = inputDirection.magnitude;
        if (debug2 != null) { debug2.transform.position = inputDirection; }

        int count = Physics.SphereCastNonAlloc(player.transform.position, 3f, inputDirection, rayCastHitArray, enemyLayerMask);
        for(int i = 0; i < count; i++)
        {
            RaycastHit hitInfo = rayCastHitArray[i];
            if(hitInfo.collider == null)
            {
                continue;
            }

            Robot robot = hitInfo.collider.GetComponentInParent<Robot>();
            if(robot != null) 
            {
                SetTarget(robot);
            }
        }
    }
}