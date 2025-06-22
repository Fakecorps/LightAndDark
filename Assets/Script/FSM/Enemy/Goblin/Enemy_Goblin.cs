public class Enemy_Goblin : Enemy
{
    #region States
    public GoblinState_Battle battleState { get; private set; }
    public GoblinState_Stun stunState { get; private set; }
    public GoblinState_KnockBack knockbackState { get; protected set; }
    public EnemyState_Attack attackState { get; protected set; }
    #endregion


    protected override void Awake()
    {
        base.Awake();

        attackState = new EnemyState_Attack(this, stateMachine, "Move", this);
        battleState = new GoblinState_Battle(this, stateMachine, "Battle", this);
        stunState = new GoblinState_Stun(this, stateMachine, "OnHit", this);
        knockbackState = new GoblinState_KnockBack(this, stateMachine, "KnockBack", this);

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        GroundCheck();
    }

}
