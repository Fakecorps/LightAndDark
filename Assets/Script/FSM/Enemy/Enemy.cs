using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyStateMachine stateMachine { get; private set; }
    [Header("Basic info")]
    public Transform PlayerTransform;
    public LayerMask PlayerLayerMask;
    public float moveSpeed;
    public float idleTime;
    [Header("Attack info")]
    public float attackCoolDown;
    [HideInInspector] public float lastTimeAttacked;//��һ�ι�����ʱ��
    public float attackDistance;//������Ҽ����С�ڴ˾���ʱ��ʼ����
    public float chaseTime;//׷��ʱ��
    [Header("Stun info")]
    public float stunDuration;
    public Vector2 stunDirection;
    public bool canBeStunned;
    [SerializeField] protected GameObject counterImage; 

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
    }

    protected override void Start()
    {
        base.Start();
        counterImage.SetActive(false);
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

    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true; 
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
}
