using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class EnemyState_Ground : EnemyState_Unprotected
{
    protected Enemy enemy;
    public EnemyState_Ground(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemyBase;
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
        if (enemy.enableChase && enemy.IsPlayerDetected())
        {
            if(enemy is Enemy_Goblin)
            {
                var goblin = enemy as Enemy_Goblin;
                stateMachine.ChangeState(goblin.attackState);
            }
            if(enemy is Enemy_Archer)
            {
                var archer = enemy as Enemy_Archer;
                stateMachine.ChangeState(archer.attackState);
            }
            
        }

    }
}
