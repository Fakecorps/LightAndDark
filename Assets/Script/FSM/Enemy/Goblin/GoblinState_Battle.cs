using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinState_Battle : EnemyState_Unprotected
{
    private Enemy_Goblin enemy;
    public GoblinState_Battle(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        enemy.ZeroVelocity();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }
}
