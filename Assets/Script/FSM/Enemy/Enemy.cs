using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyStateMachine stateMachine { get; private set; }
    [Header("Basic info")]

    public LayerMask PlayerLayerMask;
    public LayerMask DecoyLayerMask;

    public float moveSpeed;
    public float idleTime;
    [Header("Attack info")]
    public float attackCoolDown;
    [HideInInspector] public float lastTimeAttacked;//上一次攻击的时间
    public float attackDistance;//当与玩家间距离小于此距离时开始攻击
    public float chaseTime;//追击时间
    public int damage;
    [Header("Stun info")]
    public float stunDuration;
    public Vector2 stunDirection;
    public bool canBeStunned;
    [SerializeField] protected GameObject counterImage;
    [Header("AI Settings")]
    public bool enableChase = true;
    public float updateTargetInterval = 0.5f;

    public Transform PlayerTransform;
    public bool isDarkStealth;
    private float _targetUpdateTimer;

    #region States
    public EnemyState_Idle idleState { get; protected set; }
    public EnemyState_Move moveState { get; protected set; }
    public EnemyState_Ground gourndState { get; protected set; }
    public EnemyState_Dizzy dizzyState { get; protected set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        idleState = new EnemyState_Idle(this, stateMachine, "Idle");
        moveState = new EnemyState_Move(this, stateMachine, "Move");
        dizzyState = new EnemyState_Dizzy(this, stateMachine, "Dizzy");
    }

    protected override void Start()
    {
        base.Start();
        counterImage.SetActive(false);
        UpdatePlayerTarget();
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        UpdateTargetTracking();

    }
    public virtual void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheckSpot.position, Vector2.right * facingDir, 50, PlayerLayerMask);
    public virtual RaycastHit2D IsDecoyDetected() => Physics2D.Raycast(wallCheckSpot.position, Vector2.right * facingDir, 50, DecoyLayerMask);
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

    private void UpdateTargetTracking()
    {
        _targetUpdateTimer -= Time.deltaTime;
        if (_targetUpdateTimer <= 0)
        {
            UpdatePlayerTarget();
            _targetUpdateTimer = updateTargetInterval;
        }
    }

    private void UpdatePlayerTarget()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.player != null)
        {
            PlayerTransform = PlayerManager.Instance.player.transform;
        }
        else
        {
            Debug.LogWarning("Player reference not found!");
        }
    }

    public Vector2 GetPlayerDirection()
    {
        if (PlayerTransform == null) return Vector2.zero;
        return (PlayerTransform.position - transform.position).normalized;
    }

    public void ApplyStun(float duration)
    {
        // 设置眩晕持续时间
        stunDuration = duration;

        // 切换到眩晕状态
        stateMachine.ChangeState(dizzyState);
    }
}
