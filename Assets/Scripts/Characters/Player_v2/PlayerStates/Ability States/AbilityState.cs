using UnityEngine;

// Direct structural mirror of Ikenobi's AbilityState : ScriptableObject.
// Each ability (Normal, Combat, Dashing, Jumping) is a ScriptableObject that lives
// as a serialized asset on Player_v2. Player_v2 instantiates them at Start so each
// playthrough gets fresh per-instance runtime state.
//
// Responsibilities:
//   OnEnter / OnExit  — setup and teardown when ability activates/deactivates
//   InputUpdate       — ability-layer input routing (called by AbilityStateUpdater)
//   HandleMovement    — movement math, called by PlayerLocomotionManager each frame
//   HandleRotation    — rotation math, called by PlayerLocomotionManager each frame
//
// The locomotion state machine (Idle/Move/InAir/Land) remains separately and handles
// animation bool management and locomotion transitions only.
public abstract class AbilityState : ScriptableObject
{
    protected Vector3 currentVelocity;

    public virtual void OnEnter(Player_v2 player)
    {
        currentVelocity = Vector3.zero;
    }

    protected virtual void OnExit(Player_v2 player) { }

    protected virtual void InputUpdate(Player_v2 player) { }

    // Called once per frame by Player_v2.Update.
    public virtual void AbilityStateUpdater(Player_v2 player)
    {
        InputUpdate(player);
    }

    // Exits the current state and enters the next. Returns nextState so
    // Player_v2.CurrentAbilityState can be assigned in a single line.
    public AbilityState SwitchState(AbilityState nextState, Player_v2 player)
    {
        OnExit(player);
        nextState.OnEnter(player);
        return nextState;
    }

    // Called by PlayerLocomotionManager.DispatchMovement each frame.
    // States own movement math; the manager owns the call timing.
    public virtual void HandleMovement(float delta, Player_v2 player) { }
    public virtual void HandleRotation(float delta, Player_v2 player) { }
}