using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyState_Move : EnemyState_Ground
{
    public EnemyState_Move(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
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
        enemy.SetVelocity(enemy.moveSpeed * enemy.getFacingDir(), enemy.rb.velocity.y);

        if (enemy.isGroundDetected() == false)
        {
            enemy.Flip();
            enemy.ZeroVelocity();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
