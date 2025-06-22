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
    public LayerMask enemyLayer; // ȷ����Inspector����ȷ���ò㼶

    [Header("Prefabs")]
    public GameObject spikePrefab;

    private List<GameObject> activeSpikes = new List<GameObject>();
    private bool isCasting;

    protected override void Start()
    {
        Instance = this;

        // ��֤�㼶����
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("enemyLayerδ���ã���ʹ��Ĭ�ϲ㼶");
            enemyLayer = LayerMask.GetMask("Enemy"); // ���Ի�ȡ"Enemy"�㼶
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

    // �����¼����ã������ش�
    public void CreateSpikes()
    {
        if (player == null || spikePrefab == null)
        {
            Debug.LogWarning("��һ�ش�Ԥ����Ϊ��");
            return;
        }

        // ��ȡ��ҳ���
        int facingDir = player.getFacingDir();
        Vector2 playerForward = Vector2.right * facingDir;

        // �ڼ��ܷ�Χ�����ɵش�
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

                spikePos.y -= 2f; // ��ʼλ���ڵ�������

                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.identity
                );
                activeSpikes.Add(spike);
                StartCoroutine(RiseSpike(spike, spikePos + Vector2.up * 2f));
            }
        }

        // ��ⲢӰ�����
        DetectAndAffectEnemies();

        // ����ش�
        StartCoroutine(CleanupSpikes());
    }

    // �ش����𶯻�
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

    // ��ⲢӰ����� - �޸�����������
    private void DetectAndAffectEnemies()
    {
        if (player == null)
        {
            Debug.LogWarning("�������Ϊ��");
            return;
        }

        // ��ȡ��ҳ���
        int facingDir = player.getFacingDir();

        // �������������ĵ�
        Vector2 center = (Vector2)player.transform.position +
                         Vector2.right * facingDir * (skillRange / 2f);

        // 2D ��������С
        Vector2 size = new Vector2(width, 4f);

        // ȷ����������Ч
        int validLayerMask = enemyLayer.value;

        // ��ȫ��⣺�����������Ч��ʹ��Ĭ�ϵ��˲�
        if (validLayerMask == 0)
        {
            Debug.LogWarning("��Ч��enemyLayer��ʹ��Ĭ�ϵ��˲�");
            validLayerMask = LayerMask.GetMask("Enemy");
        }

        // ʹ�� Physics2D.OverlapBoxAll ���� 2D ���
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            center,
            size,
            0, // �Ƕ�
            validLayerMask
        );

        foreach (Collider2D col in hitColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                // ����˺�
                enemy.TakeDamage(damage);

                // Ӧ��ѣ��
                enemy.ApplyStun(stunDuration);

                // Ӧ�û��ˣ�ˮƽ����
                Vector2 knockbackDirection = new Vector2(facingDir, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    // ����ش�
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

    // �����жϴ���
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

    // ���ӻ�����
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