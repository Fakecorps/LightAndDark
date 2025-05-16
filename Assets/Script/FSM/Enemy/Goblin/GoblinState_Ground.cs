using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GoblinState_Ground : GoblinState_Unprotected
{
    protected Enemy_Goblin enemy;
    public GoblinState_Ground(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName,_enemy)
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
        if (enemy.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

    }
}
