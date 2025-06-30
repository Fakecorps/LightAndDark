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

    [Header("Visual Settings")]
    public GameObject chainSegmentPrefab; // 锁链段预制体
    public float segmentSize = 0.5f;     // 每个锁链段的长度
    public float chainSpeed = 10f;       // 锁链延伸速度
    public float chainWidth = 0.1f;      // 锁链宽度
    public Color chainColor = Color.white; // 锁链颜色

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // 敌人层级
    public LayerMask groundLayer;        // 地面层级

    private Enemy capturedEnemy;         // 被捕获的敌人
    private bool isPulling;              // 是否正在拉取敌人
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // 玩家朝向
    private float enemyOriginalY;        // 敌人原始高度

    // 锁链相关
    private List<GameObject> chainSegments = new List<GameObject>();
    private bool isExtending = false;    // 锁链是否正在延伸
    private bool isRetracting = false;   // 锁链是否正在收回
    private Vector3 chainTarget;         // 锁链目标位置

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

                    // 设置锁链目标位置
                    chainTarget = capturedEnemy.transform.position;

                    // 开始锁链延伸
                    StartCoroutine(ExtendChain());

                    // 开始拉取协程
                    StartCoroutine(PullEnemy());
                }
            }
        }
        else
        {
            // 没有命中敌人，锁链延伸到最大距离
            chainTarget = castPoint.position + Vector3.right * facingDir * chainRange;
            StartCoroutine(ExtendChain());
            StartCoroutine(RetractChain());
        }
    }

    // 延伸锁链
    private IEnumerator ExtendChain()
    {
        if (chainSegmentPrefab == null) yield break;

        isExtending = true;
        isRetracting = false;
        ClearChain();

        Vector3 startPos = castPoint.position;
        Vector3 direction = (chainTarget - startPos).normalized;
        float distance = Vector3.Distance(startPos, chainTarget);
        int segmentsNeeded = Mathf.CeilToInt(distance / segmentSize);

        float progress = 0f;

        while (progress < distance && isExtending)
        {
            progress += chainSpeed * Time.deltaTime;
            progress = Mathf.Min(progress, distance);

            Vector3 currentEnd = startPos + direction * progress;

            // 更新或创建锁链段
            UpdateChainSegments(startPos, currentEnd);

            yield return null;
        }

        isExtending = false;
    }

    // 更新锁链段
    private void UpdateChainSegments(Vector3 start, Vector3 end)
    {
        // 计算锁链参数
        float distance = Vector3.Distance(start, end);
        int segmentsNeeded = Mathf.CeilToInt(distance / segmentSize);
        Vector3 direction = (end - start).normalized;

        // 确保有足够的锁链段
        while (chainSegments.Count < segmentsNeeded)
        {
            GameObject segment = Instantiate(chainSegmentPrefab, castPoint.position, Quaternion.identity);
            chainSegments.Add(segment);

            // 设置锁链外观
            SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = chainColor;
                renderer.sortingOrder = 5; // 确保在角色前面
            }
        }

        // 移除多余段
        while (chainSegments.Count > segmentsNeeded)
        {
            GameObject segment = chainSegments[chainSegments.Count - 1];
            chainSegments.RemoveAt(chainSegments.Count - 1);
            Destroy(segment);
        }

        // 定位锁链段
        for (int i = 0; i < segmentsNeeded; i++)
        {
            float segmentProgress = (i + 0.5f) * segmentSize;
            if (segmentProgress > distance) segmentProgress = distance - 0.1f;

            Vector3 segmentPos = start + direction * segmentProgress;

            // 设置位置和旋转
            chainSegments[i].transform.position = segmentPos;
            chainSegments[i].transform.right = direction;

            // 设置缩放
            chainSegments[i].transform.localScale = new Vector3(
                segmentSize,
                chainWidth,
                1f
            );
        }
    }

    // 收回锁链
    private IEnumerator RetractChain()
    {
        isRetracting = true;
        isExtending = false;

        float progress = Vector3.Distance(castPoint.position, chainTarget);

        while (progress > 0 && isRetracting)
        {
            progress -= chainSpeed * Time.deltaTime;
            progress = Mathf.Max(progress, 0);

            Vector3 currentEnd = castPoint.position + (chainTarget - castPoint.position).normalized * progress;

            // 更新锁链段
            UpdateChainSegments(castPoint.position, currentEnd);

            yield return null;
        }

        ClearChain();
        isRetracting = false;
    }

    // 清除锁链
    private void ClearChain()
    {
        foreach (GameObject segment in chainSegments)
        {
            Destroy(segment);
        }
        chainSegments.Clear();
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

            // 更新锁链目标位置
            chainTarget = capturedEnemy.transform.position;

            yield return null;
        }

        // 恢复碰撞状态
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // 确保敌人在地面上
        PlaceEnemyOnGround();

        // 开始收回锁链
        StartCoroutine(RetractChain());

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
        isExtending = false;

        // 开始收回锁链
        StartCoroutine(RetractChain());

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
        if (isPulling || isExtending)
        {
            StopAllCoroutines();
            player.SetCastingSkill(false);
            CancelPull();
        }
    }
}