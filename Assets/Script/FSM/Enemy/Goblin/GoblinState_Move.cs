using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GoblinState_Move : EnemyState
{
    private Enemy_Goblin enemy;
    public GoblinState_Move(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
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
        enemy.SetVelocity(enemy.moveSpeed * enemy.getFacingDir(), enemy.rb.velocity.y);

        if (enemy.isGroundDetected() == false)
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }

    }
}
