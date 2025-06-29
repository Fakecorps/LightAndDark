using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DBossState_Attack : EnemyState_Unprotected
{
    private Enemy_DarkBoss enemy;
    private int moveDir;
    private float chaseTimer;
    private Transform PlayerTransform;
    private Transform TargetTransform;
    private Vector3 v;
    private RaycastHit2D isTargetDetected;
    public DBossState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_DarkBoss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

   
    public override void Enter()
    {
        
        base.Enter();
        dboss = GameObject.Find("DBossAnimator");
        v = dboss.transform.localScale;
        PlayerTransform = enemy.PlayerTransform;
        chaseTimer = Time.time;
        dboss.transform.localScale = new Vector3(0, 0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        yield return null;
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
            if (Mathf.Abs(TargetTransform.position.x - enemy.transform.position.x) < 5)
                moveDir = 0;
        }
        else if (TargetTransform.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
            if (Mathf.Abs(TargetTransform.position.x - enemy.transform.position.x) < 5)
                moveDir = 0;
        }
        enemy.SetVelocity(enemy.moveSpeed * moveDir*2, enemy.rb.velocity.y);

        if (isTargetDetected.distance < enemy.attackDistance && Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            dboss.transform.localScale = v;
            if(Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown + 3.5f)
                stateMachine.ChangeState(enemy.battleState);
        }


    }
}
