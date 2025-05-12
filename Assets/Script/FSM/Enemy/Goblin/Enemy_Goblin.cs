using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Goblin : Enemy
{
    #region States
    public GoblinState_Idle idleState { get; private set; }
    public GoblinState_Move moveState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        idleState = new GoblinState_Idle(this,stateMachine,"Idle",this);
        moveState = new GoblinState_Move(this, stateMachine, "Move", this);

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
