using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Dizzy : EnemyState
{
    protected Enemy enemy;
    public float DizzyDuration;
    public float DizzyTimer;               
    public EnemyState_Dizzy(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemyBase;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        DizzyDuration = Skill_D_03.Instance.DizzyDuration;
        DizzyTimer = DizzyDuration;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        DizzyTimer -= Time.deltaTime;
        if (DizzyTimer <= 0)
        { 
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
}
