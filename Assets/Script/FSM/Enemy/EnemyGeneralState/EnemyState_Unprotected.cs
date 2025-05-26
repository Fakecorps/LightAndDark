using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Unprotected : EnemyState
{
    private Enemy enemy;
    public EnemyState_Unprotected(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
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
        if (enemy.isOnHit && !enemy.canBeStunned)
        {
            enemy.isOnHit = false;
        }
        else if (enemy.isOnHit && enemy.canBeStunned)
        {
            if(enemy is Enemy_Goblin)
            {
                var goblin = enemy as Enemy_Goblin;
                stateMachine.ChangeState(goblin.stunState);
            }
            
        }
    }
}
