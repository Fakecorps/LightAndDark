using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    public ArcherState_Attack attackState {  get; private set; }
    public ArcherState_Battle battleState { get; private set; }

    protected override void Awake()
    {

        base.Awake();

        attackState = new ArcherState_Attack(this, stateMachine, "Move", this);
        battleState = new ArcherState_Battle(this, stateMachine, "Battle", this);
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
