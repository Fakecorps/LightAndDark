using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_03 : Skill
{
    public static Skill_L_03 Instance;

    [Header("Skill Settings")]
    public int damage = 60;                   // 技能伤害
    public float stunDuration = 1.5f;         // 眩晕时间
    public float knockbackForce = 10f;        // 击退力量
    public float skillRange = 5f;             // 技能作用范围
    public float width = 2f;                  // 技能宽度
    public float spikeRiseDuration = 0.5f;    // 地刺升起持续时间
    public LayerMask enemyLayer;              // 敌人层级

    [Header("Prefabs")]
    public GameObject spikePrefab;            // 地刺预制体

    private List<GameObject> activeSpikes = new List<GameObject>(); // 生成的地刺
    private bool isCasting;                   // 是否正在施法

    protected override void Start()
    {
        Instance = this;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // 触发动画
        player.anim.SetTrigger("Skill_L_03");

        // 设置施法状态
        isCasting = true;
        player.SetCastingSkill(true);

        Debug.Log("地崩圣刺技能激活");
    }

    // 动画事件调用：创建地刺
    public void CreateSpikes()
    {
        if (player == null || spikePrefab == null)
        {
            Debug.LogWarning("玩家或地刺预制体为空");
            return;
        }

        // 获取玩家朝向
        int facingDir = player.getFacingDir();
        Vector2 playerForward = Vector2.right * facingDir;

        // 在技能范围内生成地刺
        int rows = 3; // 纵向排数
        int columns = 5; // 横向列数

        for (int row = 0; row < rows; row++)
        {
            float rowDistance = (row + 1) * (skillRange / rows);

            for (int col = 0; col < columns; col++)
            {
                // 横向偏移（从中心向两侧扩展）
                float colOffset = (col - columns / 2) * (width / columns);

                // 计算地刺位置
                Vector2 spikePos = (Vector2)player.transform.position +
                                   playerForward * rowDistance +
                                   Vector2.up * colOffset;

                // 地刺初始位置在地面以下
                spikePos.y -= 2f;

                // 创建地刺
                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.identity
                );
                activeSpikes.Add(spike);

                // 启动升起动画
                StartCoroutine(RiseSpike(spike, spikePos + Vector2.up * 2f));
            }
        }

        // 检测并影响敌人
        DetectAndAffectEnemies();

        // 清理地刺
        StartCoroutine(CleanupSpikes());
    }

    // 地刺升起动画
    private IEnumerator RiseSpike(GameObject spike, Vector2 targetPosition)
    {
        if (spike == null) yield break;

        Vector2 startPosition = spike.transform.position;
        float timer = 0f;

        while (timer < spikeRiseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / spikeRiseDuration;

            if (spike != null)
            {
                spike.transform.position = Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );
            }

            yield return null;
        }
    }

    // 检测并影响敌人
    private void DetectAndAffectEnemies()
    {
        if (player == null)
        {
            Debug.LogWarning("玩家引用为空");
            return;
        }

        // 获取玩家朝向
        int facingDir = player.getFacingDir();

        // 计算检测区域中心点
        Vector2 center = (Vector2)player.transform.position +
                         Vector2.right * facingDir * (skillRange / 2f);

        // 2D 检测区域大小
        Vector2 size = new Vector2(width, 4f);

        Debug.Log($"检测区域: 中心={center}, 大小={size}, 层级={LayerMask.LayerToName(enemyLayer.value)}");

        // 使用 Physics2D.OverlapBoxAll 进行 2D 检测
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            center,
            size,
            0, // 角度
            enemyLayer
        );

        Debug.Log($"检测到 {hitColliders.Length} 个敌人碰撞体");

        foreach (Collider2D col in hitColliders)
        {
            Debug.Log($"检测到碰撞体: {col.gameObject.name}");

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"对敌人 {enemy.gameObject.name} 造成伤害");

                // 造成伤害
                enemy.TakeDamage(damage);

                // 应用眩晕
                enemy.ApplyStun(stunDuration);

                // 应用击退（水平方向）
                Vector2 knockbackDirection = new Vector2(facingDir, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
            else
            {
                Debug.LogWarning($"碰撞体 {col.gameObject.name} 没有 Enemy 组件");
            }
        }
    }

    // 清理地刺
    private IEnumerator CleanupSpikes()
    {
        // 等待地刺停留一段时间
        yield return new WaitForSeconds(1f);

        // 销毁所有地刺
        foreach (GameObject spike in activeSpikes)
        {
            if (spike != null)
            {
                Destroy(spike);
            }
        }
        activeSpikes.Clear();

        Debug.Log("地刺清理完成");

        // 结束施法状态
        player.SetCastingSkill(false);
        isCasting = false;
    }

    // 技能中断处理
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            // 销毁所有地刺
            foreach (GameObject spike in activeSpikes)
            {
                if (spike != null) Destroy(spike);
            }
            activeSpikes.Clear();

            player.SetCastingSkill(false);
            isCasting = false;

            Debug.Log("技能中断");
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        // 获取玩家朝向
        int facingDir = player.getFacingDir();

        // 计算检测区域中心点
        Vector2 center = (Vector2)player.transform.position +
                         Vector2.right * facingDir * (skillRange / 2f);

        // 绘制 2D 检测区域
        Vector3 size3D = new Vector3(width, 4f, 0.1f);
        Vector3 center3D = new Vector3(center.x, center.y, player.transform.position.z);

        Gizmos.DrawCube(center3D, size3D);
    }
}