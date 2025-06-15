using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_03 : Skill
{
    public static Skill_L_03 Instance;

    [Header("Skill Settings")]
    public int damage = 60;                   // �����˺�
    public float stunDuration = 1.5f;         // ѣ��ʱ��
    public float knockbackForce = 10f;        // ��������
    public float skillRange = 5f;             // �������÷�Χ
    public float width = 2f;                  // ���ܿ��
    public float spikeRiseDuration = 0.5f;    // �ش��������ʱ��
    public LayerMask enemyLayer;              // ���˲㼶

    [Header("Prefabs")]
    public GameObject spikePrefab;            // �ش�Ԥ����

    private List<GameObject> activeSpikes = new List<GameObject>(); // ���ɵĵش�
    private bool isCasting;                   // �Ƿ�����ʩ��

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

        // ��������
        player.anim.SetTrigger("Skill_L_03");

        // ����ʩ��״̬
        isCasting = true;
        player.SetCastingSkill(true);

        Debug.Log("�ر�ʥ�̼��ܼ���");
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
        int rows = 3; // ��������
        int columns = 5; // ��������

        for (int row = 0; row < rows; row++)
        {
            float rowDistance = (row + 1) * (skillRange / rows);

            for (int col = 0; col < columns; col++)
            {
                // ����ƫ�ƣ���������������չ��
                float colOffset = (col - columns / 2) * (width / columns);

                // ����ش�λ��
                Vector2 spikePos = (Vector2)player.transform.position +
                                   playerForward * rowDistance +
                                   Vector2.up * colOffset;

                // �ش̳�ʼλ���ڵ�������
                spikePos.y -= 2f;

                // �����ش�
                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.identity
                );
                activeSpikes.Add(spike);

                // �������𶯻�
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

    // ��ⲢӰ�����
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

        Debug.Log($"�������: ����={center}, ��С={size}, �㼶={LayerMask.LayerToName(enemyLayer.value)}");

        // ʹ�� Physics2D.OverlapBoxAll ���� 2D ���
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            center,
            size,
            0, // �Ƕ�
            enemyLayer
        );

        Debug.Log($"��⵽ {hitColliders.Length} ��������ײ��");

        foreach (Collider2D col in hitColliders)
        {
            Debug.Log($"��⵽��ײ��: {col.gameObject.name}");

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"�Ե��� {enemy.gameObject.name} ����˺�");

                // ����˺�
                enemy.TakeDamage(damage);

                // Ӧ��ѣ��
                enemy.ApplyStun(stunDuration);

                // Ӧ�û��ˣ�ˮƽ����
                Vector2 knockbackDirection = new Vector2(facingDir, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
            else
            {
                Debug.LogWarning($"��ײ�� {col.gameObject.name} û�� Enemy ���");
            }
        }
    }

    // ����ش�
    private IEnumerator CleanupSpikes()
    {
        // �ȴ��ش�ͣ��һ��ʱ��
        yield return new WaitForSeconds(1f);

        // �������еش�
        foreach (GameObject spike in activeSpikes)
        {
            if (spike != null)
            {
                Destroy(spike);
            }
        }
        activeSpikes.Clear();

        Debug.Log("�ش��������");

        // ����ʩ��״̬
        player.SetCastingSkill(false);
        isCasting = false;
    }

    // �����жϴ���
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            // �������еش�
            foreach (GameObject spike in activeSpikes)
            {
                if (spike != null) Destroy(spike);
            }
            activeSpikes.Clear();

            player.SetCastingSkill(false);
            isCasting = false;

            Debug.Log("�����ж�");
        }
    }

    // ���ӻ�����
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        // ��ȡ��ҳ���
        int facingDir = player.getFacingDir();

        // �������������ĵ�
        Vector2 center = (Vector2)player.transform.position +
                         Vector2.right * facingDir * (skillRange / 2f);

        // ���� 2D �������
        Vector3 size3D = new Vector3(width, 4f, 0.1f);
        Vector3 center3D = new Vector3(center.x, center.y, player.transform.position.z);

        Gizmos.DrawCube(center3D, size3D);
    }
}