using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_05 : Skill
{
    public static Skill_L_05 Instance;

    [Header("Area Settings")]
    public GameObject holyAreaPrefab;    // ʥ������Ԥ����
    public float areaRadius = 5f;        // ����뾶
    public float castTime = 2f;          // ʩ��ʱ�䣨����ʱ�䣩

    [Header("Spike Settings (New)")]
    public GameObject spikePrefab;       // �ش�Ԥ���壨�뼼��3��ͬ��
    public float damageDelay = 0.4f;     // �˺��ӳ�ʱ�䣨�뼼��3��ͬ��
    public float animationDuration = 1.2f; // ������ʱ�����뼼��3��ͬ��
    public int damage = 60;              // �˺�ֵ���뼼��3��ͬ��
    public float stunDuration = 1.5f;    // ѣ��ʱ�䣨�뼼��3��ͬ��
    public float knockbackForce = 10f;   // ���������뼼��3��ͬ��
    public Vector2 areaSize = new Vector2(5f, 2f); // ��������С�����ߣ�
    public Vector2 areaOffset = new Vector2(2.5f, 0f); // �������ĵ�ƫ��

    [Header("Visualization")]
    public bool showGizmo = true;        // �Ƿ���ʾ�������

    private GameObject castEffect;       // ʩ����Ч
    private bool isCasting;              // �Ƿ�����ʩ��
    private List<GameObject> activeSpikes = new List<GameObject>(); // ��ǰ����ĵش�

    protected override void Start()
    {
        Instance = this;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // ��������
        player.anim.SetTrigger("Skill_L_05");

        // ����ʩ��״̬
        player.SetCastingSkill(true);
        isCasting = true;

        // ��ʼʩ��Э��
        StartCoroutine(CastRoutine());
    }

    private IEnumerator CastRoutine()
    {
        // ʩ������ʱ��
        yield return new WaitForSeconds(castTime);

        // ʩ������������˫��ش�
        SpawnSpikes();

        // ��ʼ�˺����Э��
        StartCoroutine(DamageSequence());

        // ����ش�
        StartCoroutine(CleanupSpikes());
    }

    // ����˫��ش�
    private void SpawnSpikes()
    {
        if (spikePrefab == null)
        {
            Debug.LogWarning("�ش�Ԥ����δ����");
            return;
        }

        // ������еش�
        foreach (var spike in activeSpikes)
        {
            if (spike != null) Destroy(spike);
        }
        activeSpikes.Clear();

        // ����ǰ���ش�
        CreateSpike(1); // ǰ��

        // �����󷽵ش�
        CreateSpike(-1); // ��
    }

    // ���������ش�
    private void CreateSpike(int direction)
    {
        // ����ش�λ��
        Vector3 spawnPosition = player.transform.position;
        spawnPosition.x += areaOffset.x * direction;
        spawnPosition.y += areaOffset.y;

        // �����ش�ʵ��
        GameObject spike = Instantiate(
            spikePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // ���÷���
        Vector3 scale = spike.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        spike.transform.localScale = scale;

        activeSpikes.Add(spike);
    }

    // �˺�����
    private IEnumerator DamageSequence()
    {
        // �ȴ��˺�����ʱ��
        yield return new WaitForSeconds(damageDelay);

        // ִ���˺���⣨ǰ����������
        DetectAndAffectEnemies(1); // ǰ��
        DetectAndAffectEnemies(-1); // ��
    }

    // ��ⲢӰ����ˣ�ָ������
    private void DetectAndAffectEnemies(int direction)
    {
        if (player == null) return;

        // �������������ĵ�
        Vector2 center = (Vector2)player.transform.position;
        center.x += areaOffset.x * direction;
        center.y += areaOffset.y;

        // ��������ڵĵ���
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
                // ����˺�
                enemy.TakeDamage(damage);

                // Ӧ��ѣ��
                enemy.ApplyStun(stunDuration);

                // Ӧ�û��ˣ�ˮƽ����
                Vector2 knockbackDirection = new Vector2(direction, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    // ����ش�
    private IEnumerator CleanupSpikes()
    {
        // �ȴ���������
        yield return new WaitForSeconds(animationDuration - damageDelay);

        foreach (var spike in activeSpikes)
        {
            if (spike != null) Destroy(spike);
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

            foreach (var spike in activeSpikes)
            {
                if (spike != null) Destroy(spike);
            }
            activeSpikes.Clear();

            player.SetCastingSkill(false);
            isCasting = false;
        }
    }

    // ���ӻ����� - ���ռ���3���
    void OnDrawGizmosSelected()
    {
        if (!showGizmo || player == null) return;

        // ����ǰ���������
        DrawDetectionArea(1, Color.red);

        // ���ƺ󷽼������
        DrawDetectionArea(-1, Color.blue);
    }

    // ����ָ������ļ������
    private void DrawDetectionArea(int direction, Color color)
    {
        // �������������ĵ�
        Vector3 center = player.transform.position;
        center.x += areaOffset.x * direction;
        center.y += areaOffset.y;

        // ���Ƽ������
        Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
        Gizmos.DrawCube(center, new Vector3(areaSize.x, areaSize.y, 0.1f));

        // ���Ʒ���ָʾ
        Gizmos.color = color;
        Vector3 start = center;
        Vector3 end = start + Vector3.right * areaSize.x * 0.5f * direction;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);

        // ���Ʒ�������
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