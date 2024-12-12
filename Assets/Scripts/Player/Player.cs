using UnityEngine;

public class Player : MonoBehaviour
{
    //Unity Compenets
    public Animator animator { get; private set; }

    //Created Components
    public PlayerStatistics playerStatistics { get; private set; }

    //Status
    public bool isSprinting;
    public bool performingAction;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerStatistics = GetComponent<PlayerStatistics>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        playerStatistics.RegenerateEndurance(delta);
    }

    private void LateUpdate()
    {
        isSprinting = IsSprinting();
        performingAction = animator.GetBool("PerformingAction");
    }

    private bool IsSprinting()
    {
        return (animator.GetFloat("m_Speed") > 0.65f);
    }
}
