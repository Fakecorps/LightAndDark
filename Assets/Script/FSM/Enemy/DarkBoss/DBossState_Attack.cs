using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBossState_Attack : EnemyState_Unprotected
{
    private Enemy_DarkBoss enemy;
    private int moveDir;
    private float chaseTimer;
    private Transform PlayerTransform;
    private Transform TargetTransform;
    private GameObject dboss;
    private RaycastHit2D isTargetDetected;
    public DBossState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_DarkBoss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    void Start()
    {
        dboss = GameObject.Find("DarkBoss");
    }
    public override void Enter()
    {
        base.Enter();
        PlayerTransform = enemy.PlayerTransform;
        chaseTimer = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void Update()
    {
        Debug.Log("a");
        base.Update();
        TargetTransform = PlayerTransform;
        isTargetDetected = enemy.IsPlayerDetected();

        if (TargetTransform.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (TargetTransform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir*2, enemy.rb.velocity.y);

        if (Mathf.Abs(isTargetDetected.distance - enemy.attackDistance) < 5 && Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            stateMachine.ChangeState(enemy.battleState);
        }


    }
}
