using UnityEngine;

[CreateAssetMenu(fileName = "SounfEffects", menuName = "Scriptable Objects/SounfEffects")]
public class SounfEffects : ScriptableObject
{
    [Header("Player")]
    public AudioClip[] playerFootStep;
    public AudioClip playerPunch;
    public AudioClip playerJump;
    public AudioClip interactWithDoor;
    public AudioClip interactWithPhoneBooth;
    public AudioClip interactWithNpc;

    [Header("Enemies")]
    public AudioClip enemyDieEffect;
    public AudioClip enemyHitEffect;

    [Header("UI")]
    public AudioClip buttonClick;

}
