using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GoblinState_Attack : EnemyState
{
    private Enemy_Goblin enemy;
    public GoblinState_Attack(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemy, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
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
        if (!enemy.canAttack)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
