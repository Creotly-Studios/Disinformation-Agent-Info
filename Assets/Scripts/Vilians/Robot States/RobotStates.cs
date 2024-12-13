using UnityEngine;

public abstract class RobotStates : ScriptableObject
{
    protected Vector3 enemyDestination;

    public abstract RobotStates RobotState_Update(Robot robot);

    public RobotStates SwitchState(RobotStates nextState, Robot robot)
    {
        ResetStateParameters(robot);
        return nextState;
    }

    protected virtual void ResetStateParameters(Robot robot)
    {
        enemyDestination = Vector3.zero;
    }
}
