using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoblinState_Stun : EnemyState
{
    private Enemy_Goblin enemy;
    public GoblinState_Stun(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.isOnHit = true;
        stateTimer = enemy.stunDuration;
        enemy.SetVelocity(-enemy.getFacingDir() * enemy.stunDirection.x, enemy.rb.velocity.y+enemy.stunDirection.y);
        enemy.CloseCounterAttackWindow();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isOnHit = false;
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
