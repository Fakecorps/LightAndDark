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
    public float skillRange = 5f;
    public float width = 2f;
    public float spikeRiseDuration = 0.5f;

    [Header("Layer Settings")]
    public LayerMask enemyLayer; // 确保在Inspector中正确设置层级

    [Header("Prefabs")]
    public GameObject spikePrefab;

    private List<GameObject> activeSpikes = new List<GameObject>();
    private bool isCasting;

    protected override void Start()
    {
        Instance = this;

        // 验证层级设置
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("enemyLayer未设置，将使用默认层级");
            enemyLayer = LayerMask.GetMask("Enemy"); // 尝试获取"Enemy"层级
        }
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

        player.anim.SetTrigger("Skill_L_03");
        isCasting = true;
        player.SetCastingSkill(true);
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
        int rows = 3;
        int columns = 5;

        for (int row = 0; row < rows; row++)
        {
            float rowDistance = (row + 1) * (skillRange / rows);

            for (int col = 0; col < columns; col++)
            {
                float colOffset = (col - columns / 2) * (width / columns);
                Vector2 spikePos = (Vector2)player.transform.position +
                                   playerForward * rowDistance +
                                   Vector2.up * colOffset;

                spikePos.y -= 2f; // 初始位置在地面以下

                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.identity
                );
                activeSpikes.Add(spike);
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

    // 检测并影响敌人 - 修复层索引问题
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

        // 确保层掩码有效
        int validLayerMask = enemyLayer.value;

        // 安全检测：如果层掩码无效，使用默认敌人层
        if (validLayerMask == 0)
        {
            Debug.LogWarning("无效的enemyLayer，使用默认敌人层");
            validLayerMask = LayerMask.GetMask("Enemy");
        }

        // 使用 Physics2D.OverlapBoxAll 进行 2D 检测
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            center,
            size,
            0, // 角度
            validLayerMask
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
    private IEnumerator CleanupSpikes()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject spike in activeSpikes)
        {
            if (spike != null)
            {
                Destroy(spike);
            }
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

            foreach (GameObject spike in activeSpikes)
            {
                if (spike != null) Destroy(spike);
            }
            activeSpikes.Clear();

            player.SetCastingSkill(false);
            isCasting = false;
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        int facingDir = player.getFacingDir();
        Vector2 center = (Vector2)player.transform.position +
                         Vector2.right * facingDir * (skillRange / 2f);

        Vector3 size3D = new Vector3(width, 4f, 0.1f);
        Vector3 center3D = new Vector3(center.x, center.y, player.transform.position.z);

        Gizmos.DrawCube(center3D, size3D);
    }
}