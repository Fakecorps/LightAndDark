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
    // 添加眩晕状态管理
    private bool isStunned;
    private float stunTimer;
    private Coroutine stunRoutine;
    [SerializeField] protected GameObject counterImage;
    [Header("AI Settings")]
    public bool enableChase = true;
    public float updateTargetInterval = 0.5f;

    public Transform PlayerTransform;
    public bool isDarkStealth;
    private float _targetUpdateTimer;
    [Header("Controlled info")]
    private bool isControlled = false;

    private bool isKnockback;
    private float knockbackEndTime;


    public virtual void SetControlledState(bool state)
    {
        isControlled = state;

        if (isControlled)
        {
            // 受控时停止移动
            ZeroVelocity();
            stateMachine.ChangeState(controlState);
            anim.speed = 0.3f; // 慢动作效果

        }
        else
        {
            anim.speed = 1f; // 恢复正常
            stateMachine.ChangeState(idleState);
        }
    }

    #region States
    public EnemyState_Idle idleState { get; protected set; }
    public EnemyState_Move moveState { get; protected set; }
    public EnemyState_Ground gourndState { get; protected set; }
    public EnemyState_Dizzy dizzyState { get; protected set; }
    public EnemyState_Controlled controlState { get; protected set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        idleState = new EnemyState_Idle(this, stateMachine, "Idle");
        moveState = new EnemyState_Move(this, stateMachine, "Move");
        dizzyState = new EnemyState_Dizzy(this, stateMachine, "Dizzy");
        controlState = new EnemyState_Controlled(this, stateMachine, "Controlled");
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
        // 检查击退状态
        if (isKnockback && Time.time >= knockbackEndTime)
        {
            StopKnockback();
        }

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

// 应用眩晕
    public void ApplyStun(float duration)
    {
        // 如果已有眩晕，先结束之前的
        if (isStunned)
        {
            StopCoroutine(stunRoutine);
        }

        isStunned = true;
        stunTimer = duration;

        // 切换到眩晕状态
        stateMachine.ChangeState(dizzyState);

        // 开始眩晕计时
        stunRoutine = StartCoroutine(StunRoutine());

        Debug.Log($"{gameObject.name} 被眩晕 {duration} 秒");
    }

    // 结束眩晕
    public void EndStun()
    {
        if (isStunned)
        {
            StopCoroutine(stunRoutine);
            isStunned = false;
            Debug.Log($"{gameObject.name} 眩晕提前结束");
        }
    }

    private IEnumerator StunRoutine()
    {
        yield return new WaitForSeconds(stunTimer);
        isStunned = false;
        Debug.Log($"{gameObject.name} 眩晕结束");
    }

    //被击退功能，若重复请告知--安
    public void ApplyKnockback(Vector3 force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 确保不会过度影响垂直方向
            force.y = Mathf.Clamp(force.y, 0, force.magnitude * 0.3f);

            rb.AddForce(force, ForceMode.Impulse);

            // 记录击退状态
            isKnockback = true;
            knockbackEndTime = Time.time + 0.5f; // 短时间后结束击退
        }
    }

    // 停止击退
    public void StopKnockback()
    {
        isKnockback = false;
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
