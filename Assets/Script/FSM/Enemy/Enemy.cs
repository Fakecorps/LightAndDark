using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyStateMachine stateMachine { get; private set; }
    public LayerMask PlayerLayerMask;
    public float moveSpeed;
    public float idleTime;
    public bool canAttack;
    public CircleCollider2D Attack_Detect;
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
       stateMachine.currentState.Update();
    }
}
