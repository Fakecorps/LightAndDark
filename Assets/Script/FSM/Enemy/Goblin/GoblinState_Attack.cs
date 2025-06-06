using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GoblinState_Attack : EnemyState_Unprotected
{
    private Enemy_Goblin enemy;
    private int moveDir;
    private float chaseTimer;
    private Transform PlayerTransform;
    private Transform DecoyTransform;
    private Transform TargetTransform;

    private RaycastHit2D isTargetDetected;

    public GoblinState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        PlayerTransform = enemy.PlayerTransform;
        DecoyTransform = Skill_D_03.Instance.DecoyTransform;
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

        SetChaseTarget(Skill_D_03.Instance.isStealthed);

        if (TargetTransform.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (TargetTransform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir, enemy.rb.velocity.y);

        if (isTargetDetected.distance < enemy.attackDistance && Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

        if (!isTargetDetected && Time.time - chaseTimer > enemy.chaseTime)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    private void SetChaseTarget(bool isDarkStealth)
    {
        TargetTransform = isDarkStealth ? DecoyTransform : PlayerTransform;
        isTargetDetected = isDarkStealth ? enemy.IsDecoyDetected() : enemy.IsPlayerDetected();
    }
}

