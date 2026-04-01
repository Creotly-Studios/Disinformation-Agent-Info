public class PlayerStateMachine
{
    [field: UnityEngine.SerializeField] public PlayerState CurrentState  { get; private set; }
    [field: UnityEngine.SerializeField] public PlayerState PreviousState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
