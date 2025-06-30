using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_03 : Skill
{
    public static Skill_L_03 Instance;

    [Header("Skill Settings")]
    public int damage = 60;
    public float stunDuration = 1.5f;
    public float knockbackForce = 10f;
    public float animationDuration = 1.2f; // 动画总时长

    [Header("Detection Area")]
    public Vector2 areaSize = new Vector2(5f, 2f); // 检测区域大小 (宽, 高)
    public Vector2 areaOffset = new Vector2(2.5f, 0f); // 区域中心点偏移 (基于玩家位置)
    public bool showGizmo = true; // 是否显示检测区域

    [Header("Prefabs")]
    public GameObject spikePrefab; // 带动画的地刺预制体

    [Header("Timing Settings")]
    public float damageDelay = 0.4f; // 伤害延迟时间 (匹配动画关键帧)

    private bool isCasting;
    private GameObject activeSpike; // 当前激活的地刺

    protected override void Start()
    {
        Instance = this;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        player.anim.SetTrigger("Skill_L_03");
        isCasting = true;
        player.SetCastingSkill(true);

        // 生成地刺
        SpawnSpike();
    }

    // 生成地刺
    private void SpawnSpike()
    {
        if (spikePrefab == null)
        {
            Debug.LogWarning("地刺预制体未设置");
            return;
        }

        // 计算地刺位置
        int facingDir = player.getFacingDir();
        Vector3 spawnPosition = player.transform.position;
        spawnPosition.x += areaOffset.x * facingDir;
        spawnPosition.y += areaOffset.y;

        // 创建地刺实例
        activeSpike = Instantiate(
            spikePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 设置方向
        Vector3 scale = activeSpike.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDir;
        activeSpike.transform.localScale = scale;

        // 开始伤害检测协程
        StartCoroutine(DamageSequence());

        // 清理地刺
        StartCoroutine(CleanupSpike());
    }

    // 伤害序列
    private IEnumerator DamageSequence()
    {
        // 等待伤害触发时机
        yield return new WaitForSeconds(damageDelay);

        // 执行伤害检测
        DetectAndAffectEnemies();
    }

    // 检测并影响敌人
    private void DetectAndAffectEnemies()
    {
        if (player == null) return;

        // 获取玩家朝向
        int facingDir = player.getFacingDir();

        // 计算检测区域中心点
        Vector2 center = (Vector2)player.transform.position;
        center.x += areaOffset.x * facingDir;
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
                Vector2 knockbackDirection = new Vector2(facingDir, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    // 清理地刺
    private IEnumerator CleanupSpike()
    {
        // 等待动画结束
        yield return new WaitForSeconds(animationDuration - damageDelay);

        if (activeSpike != null)
        {
            Destroy(activeSpike);
            activeSpike = null;
        }

        player.SetCastingSkill(false);
        isCasting = false;
    }

    // 技能中断处理
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            if (activeSpike != null)
            {
                Destroy(activeSpike);
                activeSpike = null;
            }

            player.SetCastingSkill(false);
            isCasting = false;
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (!showGizmo || player == null) return;

        // 获取玩家朝向
        int facingDir = player.getFacingDir();

        // 计算检测区域中心点
        Vector3 center = player.transform.position;
        center.x += areaOffset.x * facingDir;
        center.y += areaOffset.y;

        // 绘制检测区域
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawCube(center, new Vector3(areaSize.x, areaSize.y, 0.1f));

        // 绘制方向指示
        Gizmos.color = Color.red;
        Vector3 start = center;
        Vector3 end = start + Vector3.right * areaSize.x * 0.5f * facingDir;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}