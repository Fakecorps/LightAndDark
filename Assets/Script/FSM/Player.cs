using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;



public class Player : Entity
{
    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerState_idle idleState { get; private set; }
    public PlayerState_move moveState { get; private set; }
    public PlayerState_Jump jumpState { get; private set; }
    public PlayerState_Air airState { get; private set; }
    public PlayerState_PrimaryAttack primaryAttack { get; private set; }
    public PlayerState_Parry parryState { get; private set; }
    #endregion
    #region Input
    public PlayerInput inputControl { get; private set; }
    public Vector2 AxisInput;

    #endregion
    public SpriteRenderer sr;
    #region Info
    public bool isBusy;
    [Header("Move Info")]
    public float moveSpeed;
    [Header("Jump Info")]
    public float jumpForce;
    [Header("Attack Info")]
    public int Normal_Attack_Damage;
    private bool isUltActive = false;
    [Header("Parry Info")]
    public float parryDuration;
    public bool isParrying;
    public bool parrySuccess;
    [Header("Ultimate Attack Settings")]
    public float phantomAttackCD = 0.3f; // 极短的CD时间
    private float lastPhantomAttackTime = -1f; // 初始化为-1确保第一次可以攻击
    private bool CanPerformPhantomAttack()
    {
        // 检查CD时间是否已经过去
        return Time.time >= lastPhantomAttackTime + phantomAttackCD;
    }
    // 添加施法状态变量
    private bool isCastingSkill = false;

    #endregion
    public static Player ActivePlayer { get; private set; }

    private void OnEnable()
    {
        inputControl.Enable();
        ActivePlayer = this;
    }
    private void OnDisable()
    {
        inputControl.Disable();
        if (ActivePlayer == this)
            ActivePlayer = null;
    }
    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PlayerStateMachine();
        idleState = new PlayerState_idle(this, StateMachine, "Idle");
        moveState = new PlayerState_move(this, StateMachine, "Move");
        jumpState = new PlayerState_Jump(this, StateMachine, "Jump");
        airState = new PlayerState_Air(this, StateMachine, "Jump");
        primaryAttack = new PlayerState_PrimaryAttack(this, StateMachine, "Attack");
        parryState = new PlayerState_Parry(this, StateMachine, "Parry");
        inputControl = new PlayerInput();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(idleState);
        HPSystem = HealthSystemManager.PlayerHealth;
        inputControl.Player.Skill01.started += ctx => SkillManager.instance.UseSkill(0);
        inputControl.Player.Skill02.started += ctx => SkillManager.instance.UseSkill(1);
        inputControl.Player.Skill03.started += ctx => SkillManager.instance.UseSkill(2);
        inputControl.Player.Skill04.started += ctx => SkillManager.instance.UseSkill(3);
        inputControl.Player.Skill05.started += ctx => SkillManager.instance.UseSkill(4);
    }

    protected override void Update()
    {
        base.Update();

        GatherInput();
        GroundCheck();
        StateMachine.currentState.Update();
        anim.SetFloat("yVelocity", rb.velocity.y);
        if (inputControl.Player.Attack.triggered && isUltActive && CanPerformPhantomAttack())
        {
            TriggerPhantomAttack();
        }
    }

    void GatherInput()
    {
        AxisInput = inputControl.Player.Move.ReadValue<Vector2>();
    }

   
    public void AnimationTrigger()=>StateMachine.currentState.AnimationFinishTrigger();

    public bool IsAttacking()
    {
        return StateMachine.currentState == primaryAttack;
    }

    public IEnumerator BusyFor(float _seconds)
    { 
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

   public void ActivateParrySkill()
    {
        if (PlayerManager.Instance.isPlayerLight)
        { 
            parrySuccess = true;
        }
        else
        {
            parrySuccess = true;
            Skill_D_04.Instance.UseSkill();
        }
    }

    // 添加设置施法状态的方法
    public void SetCastingSkill(bool casting)
    {
        isCastingSkill = casting;

        // 禁用移动和攻击（如果需要）
        //canMove = !casting;
        //canAttack = !casting;

        // 更新动画参数（如果需要）
        //anim.SetBool("isCasting", casting);
    }

    // 添加检查施法状态的方法
    public bool IsCastingSkill()
    {
        return isCastingSkill;
    }
    private void TriggerPhantomAttack()
    {
        // 获取当前激活的UltField
        UltField activeField = FindObjectOfType<UltField>();
        if (activeField != null)
        {
            activeField.TriggerPhantomAttack();
            // 更新最后攻击时间
            lastPhantomAttackTime = Time.time;
        }
    }

    // 设置技能激活状态
    public void SetUltActiveState(bool state)
    {
        isUltActive = state;
    }
}

