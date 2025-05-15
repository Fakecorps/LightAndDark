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
    [Header("Attack info")]
    public float attackCoolDown;
    [HideInInspector] public float lastTimeAttacked;//上一次攻击的时间
    public float attackDistance;//当与玩家间距离小于此距离时开始攻击
    public float chaseTime;//追击时间
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
    public virtual void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheckSpot.position, Vector2.right * facingDir, 50, PlayerLayerMask);
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
    }
}
