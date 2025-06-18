using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Death : EnemyState
{
    private float deathDuration = 1.5f; // 死亡动画持续时间
    protected Enemy enemy;
    public EnemyState_Death(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.ZeroVelocity();
        // 禁用所有物理和碰撞
        enemy.col.enabled = false;
        enemy.rb.isKinematic = true;
        enemy.rb.velocity = Vector2.zero;

        // 触发死亡事件
        enemy.OnDeath?.Invoke();
    }

    public override void Update()
    {
        base.Update();
        enemy.ZeroVelocity();
        // 等待死亡动画播放完毕
        deathDuration -= Time.deltaTime;
        if (deathDuration <= 0)
        {
            // 延迟销毁对象
            Object.Destroy(enemy.gameObject);
        }
    }
}
