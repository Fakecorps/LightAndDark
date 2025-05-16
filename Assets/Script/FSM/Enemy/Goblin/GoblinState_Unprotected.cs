using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinState_Unprotected : EnemyState
{
    private Enemy_Goblin enemy;
    public GoblinState_Unprotected(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (enemy.isOnHit)
        {
            stateMachine.ChangeState(enemy.stunState);
        }
    }
}
