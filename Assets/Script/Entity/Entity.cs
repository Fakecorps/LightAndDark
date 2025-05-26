using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public CapsuleCollider2D col { get; private set; }
    public EntityFX fX { get; private set; }
    public HealthSystem HPSystem { get; set; }
    #endregion

    [Header("Ground Check")]
    [SerializeField] protected Transform groundCheckSpot;
    public bool isGrounded;
    [SerializeField] protected LayerMask groundLayer; // 地面层级
    [SerializeField] protected float groundCheckDistance = 0.1f;

    [Header("Facing Dir")]
    protected bool facingRight = true;
    protected int facingDir { get; private set; } = 1;

    [Header("Wall Check")]
    [SerializeField] protected Transform wallCheckSpot;

    [Header("Attack Check")]
    public Transform attackCheckSpot;
    public float attackRadius;

    [Header("OnHit info")]
    public bool isOnHit;
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        fX = GetComponentInChildren<EntityFX>();
        if (this is Enemy)
        {
            HPSystem = GetComponent<HealthSystem>();
            if (HPSystem == null) // 如果预制体未挂载则自动添加
                HPSystem = gameObject.AddComponent<HealthSystem>();
        }

    }
    protected virtual void Update()
    { 
        
    }

    public virtual void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckSpot.position, Vector2.down, groundCheckDistance, groundLayer);

        isGrounded = hit.collider != null;
    }

    public virtual bool isGroundDetected() => isGrounded;

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheckSpot.position, new Vector3(groundCheckSpot.position.x, groundCheckSpot.position.y - groundCheckDistance));
        Gizmos.DrawWireSphere(attackCheckSpot.position, attackRadius);
    }

    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual int getFacingDir() => facingDir; 

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    {
        FlipController(_xVelocity);
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    public virtual void ZeroVelocity() => rb.velocity = new Vector2(0, 0);
    public virtual void TakeDamage(int damage)
    {
        if (HPSystem == null) return;

        isOnHit = true;
        fX.StartCoroutine("FlashFX");
        HPSystem.TakeDamage(damage); // 调用HealthSystem

        Debug.Log($"{gameObject.name}受到{damage}点伤害");
    }



}
