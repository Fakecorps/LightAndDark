using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherState_Battle : EnemyState_Unprotected
{
    private Enemy_Archer enemy;
    private int moveDir;
    private float dashTimer;
    private Transform PlayerTransform;
    public ArcherState_Battle(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        PlayerTransform = enemy.PlayerTransform;
        enemy.ZeroVelocity();
        dashTimer = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
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

        if (PlayerTransform.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (PlayerTransform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir*3f, enemy.rb.velocity.y);

        if ( Time.time - dashTimer >= 3 || Mathf.Abs(enemy.transform.position.x - enemy.PlayerTransform.position.x) < 2)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }



}
