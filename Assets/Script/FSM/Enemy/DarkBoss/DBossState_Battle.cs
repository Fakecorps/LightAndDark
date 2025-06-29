using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBossState_Battle : EnemyState_Unprotected
{
    private Enemy_DarkBoss enemy;
    public DBossState_Battle(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_DarkBoss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
        Debug.Log("b");
        base.Update();
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }
}
