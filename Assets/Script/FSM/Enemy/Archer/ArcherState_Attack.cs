using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherState_Attack : EnemyState_Unprotected
{
    private Enemy_Archer enemy;
    private int moveDir;
    private float chaseTimer;
    private Transform PlayerTransform;
    private Transform DecoyTransform;
    private Transform TargetTransform;

    private RaycastHit2D isTargetDetected;
    public ArcherState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
        Debug.Log("1");

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

        if (Mathf.Abs(isTargetDetected.distance - enemy.attackDistance)< 20 && Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

        if (!isTargetDetected && Time.time - chaseTimer > enemy.chaseTime)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    private void SetChaseTarget(bool isDarkStealth)
    {
        TargetTransform = isDarkStealth ? DecoyTransform : PlayerTransform;
        isTargetDetected = isDarkStealth ? enemy.IsDecoyDetected() : enemy.IsPlayerDetected();
    }
}
