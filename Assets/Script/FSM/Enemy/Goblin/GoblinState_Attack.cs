using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GoblinState_Attack : EnemyState
{
    private Enemy_Goblin enemy;
    private int moveDir;
    private float chaseTimer;
    public GoblinState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        chaseTimer = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    void Start()
    {

    }

    public override void Update()
    {
        base.Update();

        if (Player.Instance.transform.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if(Player.Instance.transform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir,enemy.rb.velocity.y);
        
        if (enemy.IsPlayerDetected().distance<enemy.attackDistance && Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

        if (!enemy.IsPlayerDetected() && Time.time - chaseTimer > enemy.chaseTime)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
