using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_03 : Skill
{
    public static Skill_L_03 Instance;

    [Header("Skill Settings")]
    public int damage = 60;                   // 技能伤害
    public float stunDuration = 1.5f;            // 眩晕时间
    public float knockbackForce = 10f;           // 击退力量
    public float skillRange = 5f;                // 技能作用范围
    public float width = 2f;                     // 技能宽度
    public float spikeRiseDuration = 0.5f;       // 地刺升起持续时间
    public LayerMask enemyLayer;

    [Header("Prefabs")]
    public GameObject spikePrefab;               // 地刺预制体

    private List<GameObject> activeSpikes = new List<GameObject>(); // 生成的地刺
    private bool isCasting;                      // 是否正在施法

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

        // 添加调试信息
        Debug.Log("Skill_L_03 activated");
    }

    // 动画事件调用：创建地刺
    public void CreateSpikes()
    {
        if (player == null || spikePrefab == null)
            return;

        // 计算生成区域
        Vector3 playerPos = player.transform.position;
        Vector3 playerForward = player.transform.forward;

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
                Vector3 spikePos = playerPos + playerForward * rowDistance;
                spikePos += player.transform.right * colOffset;

                // 地刺初始位置在地面以下
                spikePos.y -= 2f;

                // 创建地刺
                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.LookRotation(playerForward)
                );
                activeSpikes.Add(spike);

                // 启动升起动画
                StartCoroutine(RiseSpike(spike, spikePos + Vector3.up * 2f));
            }
        }

        // 检测并影响敌人
        DetectAndAffectEnemies();

        // 清理地刺
        StartCoroutine(CleanupSpikes());
    }

    // 地刺升起动画
    private IEnumerator RiseSpike(GameObject spike, Vector3 targetPosition)
    {
        if (spike == null) yield break;

        Vector3 startPosition = spike.transform.position;
        float timer = 0f;

        while (timer < spikeRiseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / spikeRiseDuration;

            if (spike != null)
            {
                spike.transform.position = Vector3.Lerp(
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
        // 使用盒形区域检测敌人
        Vector3 center = player.transform.position + player.transform.forward * (skillRange / 2f);
        Vector3 halfExtents = new Vector3(width / 2f, 2f, skillRange / 2f);

        Collider[] hitColliders = Physics.OverlapBox(
            center,
            halfExtents,
            player.transform.rotation,
            enemyLayer
        );

        foreach (Collider col in hitColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 造成伤害
                enemy.TakeDamage(damage);

                // 应用眩晕
                enemy.ApplyStun(stunDuration);

                // 应用击退（方向为玩家前方）
                Vector3 knockbackDirection = player.transform.forward;
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
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
        }
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        // 绘制技能作用区域
        Vector3 center = player.transform.position + player.transform.forward * (skillRange / 2f);
        Vector3 size = new Vector3(width, 4f, skillRange);

        // 应用玩家旋转
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            center,
            player.transform.rotation,
            Vector3.one
        );

        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }
    protected override void Update()
    {
        base.Update();

    }
}
