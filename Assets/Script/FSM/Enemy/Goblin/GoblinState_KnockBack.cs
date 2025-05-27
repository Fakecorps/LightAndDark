using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinState_KnockBack : EnemyState
{
    private Enemy_Goblin enemy;
    public GoblinState_KnockBack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    { 
        base.Enter();
        enemy.isOnHit = true;
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
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
