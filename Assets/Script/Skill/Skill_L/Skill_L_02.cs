using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // 光链发射点
    [Range(1f, 20f)] public float chainRange = 8f; // 可滑动调整的射程
    [Range(1f, 10f)] public float pullSpeed = 5f;  // 可滑动调整的拉取速度
    public float stunDuration = 1f;      // 眩晕时间
    [Range(0.5f, 5f)] public float minDistance = 1.5f; // 可滑动调整的最小距离

    [Header("Visual Settings")]
    public GameObject chainVisualPrefab; // 带尖的锁链贴图预制体
    [Range(1f, 20f)] public float chainExtendSpeed = 10f; // 可滑动调整的延伸速度
    [Range(1f, 20f)] public float chainRetractSpeed = 15f; // 可滑动调整的收回速度
    [Range(0.1f, 2f)] public float chainWidth = 0.5f; // 可滑动调整的锁链宽度
    [Range(0f, 1f)] public float chainTipOffset = 0.1f;  // 可滑动调整的尖端偏移量

    [Header("Visualization Tools")]
    [Tooltip("在编辑器中显示可视化辅助")]
    public bool showVisualization = true;
    [Range(0f, 1f)] public float previewLength = 1f; // 预览长度比例

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // 敌人层级
    public LayerMask groundLayer;        // 地面层级

    private Enemy capturedEnemy;         // 被捕获的敌人
    private bool isPulling;              // 是否正在拉取敌人
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // 玩家朝向
    private float enemyOriginalY;        // 敌人原始高度

    // 锁链视觉相关
    private GameObject chainVisual;      // 锁链视觉对象
    private Transform chainTip;          // 锁链尖端
    private bool isExtending = false;    // 锁链是否正在延伸
    private bool isRetracting = false;   // 锁链是否正在收回
    private Vector3 chainTarget;         // 锁链目标位置
    private Vector3 chainOrigin;         // 锁链起始位置（固定）
    private float chainProgress;         // 锁链当前进度

    // 施法状态
    private bool isCasting = false;      // 是否正在施法

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // 设置玩家施法状态
        player.SetCastingSkill(true);
        isCasting = true;

        // 保存玩家朝向
        facingDir = player.getFacingDir();

        // 设置固定起始点
        chainOrigin = castPoint.position;

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
                if (capturedEnemy != null && capturedEnemy.IsAlive())
                {
                    // 保存敌人原始高度
                    enemyOriginalY = capturedEnemy.transform.position.y;

                    // 应用眩晕效果
                    capturedEnemy.ApplyStun(stunDuration);

                    // 设置锁链目标位置（严格水平方向）
                    chainTarget = new Vector3(
                        capturedEnemy.transform.position.x,
                        chainOrigin.y, // 使用与发射点相同的高度
                        capturedEnemy.transform.position.z
                    );

                    // 创建锁链视觉
                    CreateChainVisual();

                    // 开始锁链延伸
                    StartCoroutine(ExtendChain());

                    // 开始拉取协程
                    StartCoroutine(PullEnemy());
                }
            }
        }
        else
        {
            // 没有命中敌人，锁链延伸到最大距离（严格水平方向）
            chainTarget = new Vector3(
                castPoint.position.x + facingDir * chainRange,
                castPoint.position.y, // 与发射点相同高度
                castPoint.position.z
            );

            // 创建锁链视觉
            CreateChainVisual();

            // 开始延伸然后收回
            StartCoroutine(ExtendThenRetract());
        }
    }

    // 创建锁链视觉 - 使用美术提供的贴图
    private void CreateChainVisual()
    {
        if (chainVisualPrefab == null)
        {
            Debug.LogWarning("锁链视觉预制体未设置");
            return;
        }

        // 销毁现有锁链
        if (chainVisual != null)
        {
            Destroy(chainVisual);
        }

        // 创建新锁链对象
        chainVisual = Instantiate(
            chainVisualPrefab,
            chainOrigin, // 固定在发射点
            Quaternion.identity
        );

        // 获取尖端引用
        chainTip = chainVisual.transform.Find("Tip");
        if (chainTip == null && chainVisual.transform.childCount > 0)
        {
            chainTip = chainVisual.transform.GetChild(0);
        }

        // 初始缩放
        chainVisual.transform.localScale = new Vector3(0.1f, chainWidth, 1f);
        chainProgress = 0f;

        // 设置初始位置和旋转
        UpdateChainVisual();
    }

    // 更新锁链视觉 - 尾端固定，前端延伸
    private void UpdateChainVisual()
    {
        if (chainVisual == null) return;

        // 计算方向
        Vector3 direction = (chainTarget - chainOrigin).normalized;

        // 始终固定在发射点
        chainVisual.transform.position = chainOrigin;

        // 设置旋转（朝向目标）
        chainVisual.transform.right = direction;

        // 计算当前长度
        float currentLength = Vector3.Distance(chainOrigin, chainTarget) * chainProgress;

        // 设置缩放（长度和宽度）
        chainVisual.transform.localScale = new Vector3(
            currentLength, // X轴缩放控制长度
            chainWidth,    // Y轴缩放控制宽度
            1f
        );

        // 调整尖端位置
        if (chainTip != null)
        {
            // 尖端位置 = 起点 + 方向 * 当前长度
            chainTip.position = chainOrigin + direction * currentLength;
        }
    }

    // 延伸锁链
    private IEnumerator ExtendChain()
    {
        isExtending = true;
        isRetracting = false;

        float totalDistance = Vector3.Distance(chainOrigin, chainTarget);
        float currentDistance = 0f;

        while (currentDistance < totalDistance && isExtending)
        {
            currentDistance += chainExtendSpeed * Time.deltaTime;
            chainProgress = Mathf.Clamp01(currentDistance / totalDistance);

            UpdateChainVisual();
            yield return null;
        }

        chainProgress = 1f;
        UpdateChainVisual();
        isExtending = false;
    }

    // 延伸然后收回（用于未命中敌人）
    private IEnumerator ExtendThenRetract()
    {
        yield return StartCoroutine(ExtendChain());
        yield return new WaitForSeconds(0.2f); // 短暂停留
        yield return StartCoroutine(RetractChain());
    }

    // 收回锁链 - 从目标点向起点收回
    private IEnumerator RetractChain()
    {
        isRetracting = true;
        isExtending = false;

        float totalDistance = Vector3.Distance(chainOrigin, chainTarget);
        float currentDistance = totalDistance;

        while (currentDistance > 0 && isRetracting)
        {
            currentDistance -= chainRetractSpeed * Time.deltaTime;
            chainProgress = Mathf.Clamp01(currentDistance / totalDistance);

            UpdateChainVisual();
            yield return null;
        }

        // 销毁锁链
        if (chainVisual != null)
        {
            Destroy(chainVisual);
            chainVisual = null;
            chainTip = null;
        }

        // 结束施法状态
        EndSkillCasting();
        isRetracting = false;
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

        // 拉取循环 - 严格水平移动
        while (isPulling && capturedEnemy != null && capturedEnemy.IsAlive())
        {
            // 计算敌人到玩家的水平距离
            Vector3 playerPos = player.transform.position;
            Vector3 enemyPos = capturedEnemy.transform.position;

            // 严格水平距离计算（忽略Y轴）
            float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);

            // 如果距离小于最小值，停止拉取
            if (horizontalDistance <= minDistance)
            {
                break;
            }

            // 计算拉取方向（仅水平方向）
            float pullDirection = Mathf.Sign(playerPos.x - enemyPos.x);

            // 计算移动向量（仅水平移动）
            float moveAmount = pullDirection * pullSpeed * Time.deltaTime;

            // 移动敌人（仅改变X坐标）
            Vector3 newPosition = new Vector3(
                enemyPos.x + moveAmount,
                enemyOriginalY, // 严格保持原始高度
                enemyPos.z
            );

            capturedEnemy.transform.position = newPosition;

            // 更新锁链目标位置（严格水平方向，保持与发射点相同高度）
            chainTarget = new Vector3(
                newPosition.x,
                chainOrigin.y, // 使用与发射点相同的高度
                newPosition.z
            );

            // 更新锁链视觉
            UpdateChainVisual();

            yield return null;
        }

        // 恢复碰撞状态
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // 开始收回锁链
        StartCoroutine(RetractChain());

        // 结束拉取
        capturedEnemy = null;
        isPulling = false;
    }

    // 中断拉取
    public void CancelPull()
    {
        isPulling = false;
        isExtending = false;

        // 开始收回锁链
        if (chainVisual != null)
        {
            StartCoroutine(RetractChain());
        }
        else
        {
            EndSkillCasting();
        }

        if (capturedEnemy != null)
        {
            // 恢复碰撞状态
            Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }

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
        if (isCasting)
        {
            StopAllCoroutines();
            CancelPull();
            EndSkillCasting();
        }
    }

    // 可视化调整功能
    private void OnDrawGizmosSelected()
    {
        if (!showVisualization || castPoint == null) return;

        int facing = Application.isPlaying ? facingDir : 1;
        if (facing == 0) facing = 1; // 避免0方向

        // 绘制锁链最大射程
        Gizmos.color = new Color(0, 0.5f, 1f, 0.7f); // 半透明蓝色
        Vector3 endPoint = new Vector3(
            castPoint.position.x + facing * chainRange,
            castPoint.position.y,
            castPoint.position.z
        );
        Gizmos.DrawLine(castPoint.position, endPoint);

        // 绘制拉取停止距离
        Gizmos.color = new Color(1f, 1f, 0, 0.5f); // 半透明黄色
        Gizmos.DrawWireSphere(castPoint.position, minDistance);

        // 绘制预览锁链
        if (previewLength > 0)
        {
            Vector3 previewTarget = new Vector3(
                castPoint.position.x + facing * chainRange * previewLength,
                castPoint.position.y,
                castPoint.position.z
            );

            // 计算方向
            Vector3 direction = (previewTarget - castPoint.position).normalized;
            float previewDistance = Vector3.Distance(castPoint.position, previewTarget);

            // 绘制预览锁链
            Gizmos.color = new Color(0, 1f, 0, 0.8f); // 半透明绿色
            Gizmos.DrawLine(castPoint.position, previewTarget);

            // 绘制预览尖端
            Gizmos.DrawWireSphere(previewTarget, 0.1f);

            // 绘制预览宽度指示器
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            Gizmos.DrawLine(
                previewTarget - perpendicular * chainWidth * 0.5f,
                previewTarget + perpendicular * chainWidth * 0.5f
            );
        }
    }

    // 结束施法状态
    private void EndSkillCasting()
    {
        if (isCasting && player != null)
        {
            player.SetCastingSkill(false);
            isCasting = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Skill_L_02))]
public class Skill_L_02Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Skill_L_02 skill = (Skill_L_02)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("可视化预览工具", EditorStyles.boldLabel);

        // 添加预览滑块
        skill.previewLength = EditorGUILayout.Slider("锁链长度预览", skill.previewLength, 0f, 1f);

        // 添加可视化开关
        skill.showVisualization = EditorGUILayout.Toggle("显示可视化", skill.showVisualization);

        // 添加刷新按钮
        if (GUILayout.Button("刷新预览"))
        {
            // 强制重绘场景视图
            SceneView.RepaintAll();
        }

        EditorGUILayout.HelpBox("调整参数后，使用'刷新预览'按钮更新场景视图中的可视化效果。", MessageType.Info);
    }
}
#endif