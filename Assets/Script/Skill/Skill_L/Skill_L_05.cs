using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_05 : Skill
{
    public static Skill_L_05 Instance;

    [Header("Area Settings")]
    public GameObject holyAreaPrefab;    // 圣光区域预制体
    public float areaRadius = 5f;        // 区域半径
    public float castTime = 2f;          // 施法时间（引导时间）

    [Header("Spike Settings (New)")]
    public GameObject spikePrefab;       // 地刺预制体（与技能3相同）
    public float damageDelay = 0.4f;     // 伤害延迟时间（与技能3相同）
    public float animationDuration = 1.2f; // 动画总时长（与技能3相同）
    public int damage = 60;              // 伤害值（与技能3相同）
    public float stunDuration = 1.5f;    // 眩晕时间（与技能3相同）
    public float knockbackForce = 10f;   // 击退力（与技能3相同）
    public Vector2 areaSize = new Vector2(5f, 2f); // 检测区域大小（宽，高）
    public Vector2 areaOffset = new Vector2(2.5f, 0f); // 区域中心点偏移

    [Header("Visualization")]
    public bool showGizmo = true;        // 是否显示检测区域

    private GameObject castEffect;       // 施法特效
    private bool isCasting;              // 是否正在施法
    private List<GameObject> activeSpikes = new List<GameObject>(); // 当前激活的地刺

    protected override void Start()
    {
        Instance = this;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // 触发动画
        player.anim.SetTrigger("Skill_L_05");

        // 设置施法状态
        player.SetCastingSkill(true);
        isCasting = true;

        // 开始施法协程
        StartCoroutine(CastRoutine());
    }

    private IEnumerator CastRoutine()
    {
        // 施法引导时间
        yield return new WaitForSeconds(castTime);

        // 施法结束，创建双向地刺
        SpawnSpikes();

        // 开始伤害检测协程
        StartCoroutine(DamageSequence());

        // 清理地刺
        StartCoroutine(CleanupSpikes());
    }

    // 生成双向地刺
    private void SpawnSpikes()
    {
        if (spikePrefab == null)
        {
            Debug.LogWarning("地刺预制体未设置");
            return;
        }

        // 清空现有地刺
        foreach (var spike in activeSpikes)
        {
            if (spike != null) Destroy(spike);
        }
        activeSpikes.Clear();

        // 创建前方地刺
        CreateSpike(1); // 前方

        // 创建后方地刺
        CreateSpike(-1); // 后方
    }

    // 创建单个地刺
    private void CreateSpike(int direction)
    {
        // 计算地刺位置
        Vector3 spawnPosition = player.transform.position;
        spawnPosition.x += areaOffset.x * direction;
        spawnPosition.y += areaOffset.y;

        // 创建地刺实例
        GameObject spike = Instantiate(
            spikePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 设置方向
        Vector3 scale = spike.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        spike.transform.localScale = scale;

        activeSpikes.Add(spike);
    }

    // 伤害序列
    private IEnumerator DamageSequence()
    {
        // 等待伤害触发时机
        yield return new WaitForSeconds(damageDelay);

        // 执行伤害检测（前后两个方向）
        DetectAndAffectEnemies(1); // 前方
        DetectAndAffectEnemies(-1); // 后方
    }

    // 检测并影响敌人（指定方向）
    private void DetectAndAffectEnemies(int direction)
    {
        if (player == null) return;

        // 计算检测区域中心点
        Vector2 center = (Vector2)player.transform.position;
        center.x += areaOffset.x * direction;
        center.y += areaOffset.y;

        // 检测区域内的敌人
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            center,
            areaSize,
            0f,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D col in hitColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 造成伤害
                enemy.TakeDamage(damage);

                // 应用眩晕
                enemy.ApplyStun(stunDuration);

                // 应用击退（水平方向）
                Vector2 knockbackDirection = new Vector2(direction, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    // 清理地刺
    private IEnumerator CleanupSpikes()
    {
        // 等待动画结束
        yield return new WaitForSeconds(animationDuration - damageDelay);

        foreach (var spike in activeSpikes)
        {
            if (spike != null) Destroy(spike);
        }
        activeSpikes.Clear();

        player.SetCastingSkill(false);
        isCasting = false;
    }

    // 技能中断处理
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            foreach (var spike in activeSpikes)
            {
                if (spike != null) Destroy(spike);
            }
            activeSpikes.Clear();

            player.SetCastingSkill(false);
            isCasting = false;
        }
    }

    // 可视化调试 - 按照技能3风格
    void OnDrawGizmosSelected()
    {
        if (!showGizmo || player == null) return;

        // 绘制前方检测区域
        DrawDetectionArea(1, Color.red);

        // 绘制后方检测区域
        DrawDetectionArea(-1, Color.blue);
    }

    // 绘制指定方向的检测区域
    private void DrawDetectionArea(int direction, Color color)
    {
        // 计算检测区域中心点
        Vector3 center = player.transform.position;
        center.x += areaOffset.x * direction;
        center.y += areaOffset.y;

        // 绘制检测区域
        Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
        Gizmos.DrawCube(center, new Vector3(areaSize.x, areaSize.y, 0.1f));

        // 绘制方向指示
        Gizmos.color = color;
        Vector3 start = center;
        Vector3 end = start + Vector3.right * areaSize.x * 0.5f * direction;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);

        // 绘制方向文字
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 12;
        UnityEditor.Handles.Label(center + Vector3.up * 0.5f,
                                direction > 0 ? "Front" : "Back",
                                style);
#endif
    }
}