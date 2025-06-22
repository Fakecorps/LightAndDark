using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // 光链发射点
    public float chainRange = 8f;        // 光链射程
    public float pullSpeed = 5f;         // 拉取速度
    public float stunDuration = 1f;      // 眩晕时间
    public float minDistance = 1.5f;     // 拉取停止的最小距离
    public float groundHeightOffset = 0.5f; // 地面高度偏移

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // 敌人层级
    public LayerMask groundLayer;        // 地面层级

    private Enemy capturedEnemy;         // 被捕获的敌人
    private bool isPulling;              // 是否正在拉取敌人
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // 玩家朝向
    private float enemyOriginalY;        // 敌人原始高度

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // 保存玩家朝向
        facingDir = player.getFacingDir();

        // 触发动画
        player.anim.SetTrigger("Skill_L_02");
    }

    // 动画事件调用（在动画关键帧添加事件）
    public void OnCastAnimationEvent()
    {
        if (player == null) return;

        // 检测前方第一个敌人
        RaycastHit2D hit = Physics2D.Raycast(
            castPoint.position,
            Vector2.right * facingDir,
            chainRange,
            enemyLayer
        );

        // 检查是否命中敌人
        if (hit.collider != null)
        {
            // 检查是否在敌人层级
            if (enemyLayer == (enemyLayer | (1 << hit.collider.gameObject.layer)))
            {
                capturedEnemy = hit.collider.GetComponent<Enemy>();
                if (capturedEnemy != null)
                {
                    // 保存敌人原始高度
                    enemyOriginalY = capturedEnemy.transform.position.y;

                    // 应用眩晕效果
                    capturedEnemy.ApplyStun(stunDuration);

                    // 开始拉取协程
                    StartCoroutine(PullEnemy());
                }
            }
        }
    }

    private IEnumerator PullEnemy()
    {
        if (capturedEnemy == null) yield break;

        isPulling = true;

        // 保存原始碰撞状态
        Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
        bool originalColliderEnabled = enemyCollider != null ? enemyCollider.enabled : false;

        // 临时禁用碰撞避免卡住
        if (enemyCollider != null) enemyCollider.enabled = false;

        // 拉取循环
        while (isPulling && capturedEnemy != null)
        {
            // 计算敌人到玩家的水平距离
            Vector3 playerPos = player.transform.position;
            Vector3 enemyPos = capturedEnemy.transform.position;

            // 保持敌人原始高度
            enemyPos.y = enemyOriginalY;

            // 计算水平距离
            float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);

            // 如果距离小于最小值，停止拉取
            if (horizontalDistance <= minDistance)
            {
                break;
            }

            // 计算拉取方向（仅水平方向）
            Vector3 pullDirection = new Vector3(
                Mathf.Sign(playerPos.x - enemyPos.x), // 水平方向
                0, // 垂直方向设为0，保持高度不变
                0
            );

            // 计算移动向量（仅水平移动）
            Vector3 moveVector = pullDirection * pullSpeed * Time.deltaTime;

            // 移动敌人（仅改变X坐标）
            capturedEnemy.transform.position = new Vector3(
                enemyPos.x + moveVector.x,
                enemyOriginalY, // 保持原始高度
                enemyPos.z
            );

            // 绘制调试线（实际使用时可替换为视觉效果）
            Debug.DrawLine(
                castPoint.position,
                capturedEnemy.transform.position,
                Color.yellow,
                0.1f
            );

            yield return null;
        }

        // 恢复碰撞状态
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // 确保敌人在地面上
        PlaceEnemyOnGround();

        // 结束拉取
        capturedEnemy = null;
        isPulling = false;
    }

    // 确保敌人放置在地面上
    private void PlaceEnemyOnGround()
    {
        if (capturedEnemy == null) return;

        RaycastHit2D groundHit = Physics2D.Raycast(
            capturedEnemy.transform.position,
            Vector2.down,
            5f,
            groundLayer
        );

        if (groundHit.collider != null)
        {
            Vector3 newPosition = capturedEnemy.transform.position;
            newPosition.y = groundHit.point.y + groundHeightOffset;
            capturedEnemy.transform.position = newPosition;
        }
    }

    // 中断拉取
    public void CancelPull()
    {
        isPulling = false;
        if (capturedEnemy != null)
        {
            // 恢复碰撞状态
            Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }

            // 确保敌人在地面上
            PlaceEnemyOnGround();

            capturedEnemy = null;
        }
    }

    protected override void Update()
    {
        base.Update();

        // 不再需要在此更新位置，全部在协程中处理
    }

    protected override void Start()
    {
        base.Start();
        skill_L_02 = this;

        // 安全初始化
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("enemyLayer未设置，将使用默认层级");
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        // 设置地面层级
        if (groundLayer.value == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
    }

// 当技能被取消时调用
public void CancelSkill()
    {
        if (isPulling)
        {
            StopAllCoroutines();
            player.SetCastingSkill(false);
            isPulling = false;
            CancelPull();
        }
    }
}