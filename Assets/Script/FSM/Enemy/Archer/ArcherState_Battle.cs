using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherState_Battle : EnemyState_Unprotected
{
    private Enemy_Archer enemy;
    public ArcherState_Battle(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        enemy.ZeroVelocity();
        Debug.Log("4");
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("3");
        enemy.lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();
        Debug.Log("2");
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }
}
