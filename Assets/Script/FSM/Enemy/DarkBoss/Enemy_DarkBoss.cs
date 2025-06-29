using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DarkBoss : Enemy
{
    public DBossState_Attack attackState { get; private set; }
    public DBossState_Battle battleState { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        attackState = new DBossState_Attack(this, stateMachine, "Move", this);
        battleState = new DBossState_Battle(this, stateMachine, "Battle", this);

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
