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
    public float animationDuration = 1.2f; // ������ʱ��

    [Header("Detection Area")]
    public Vector2 areaSize = new Vector2(5f, 2f); // ��������С (��, ��)
    public Vector2 areaOffset = new Vector2(2.5f, 0f); // �������ĵ�ƫ�� (�������λ��)
    public bool showGizmo = true; // �Ƿ���ʾ�������

    [Header("Prefabs")]
    public GameObject spikePrefab; // �������ĵش�Ԥ����

    [Header("Timing Settings")]
    public float damageDelay = 0.4f; // �˺��ӳ�ʱ�� (ƥ�䶯���ؼ�֡)

    private bool isCasting;
    private GameObject activeSpike; // ��ǰ����ĵش�

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

        // ���ɵش�
        SpawnSpike();
    }

    // ���ɵش�
    private void SpawnSpike()
    {
        if (spikePrefab == null)
        {
            Debug.LogWarning("�ش�Ԥ����δ����");
            return;
        }

        // ����ش�λ��
        int facingDir = player.getFacingDir();
        Vector3 spawnPosition = player.transform.position;
        spawnPosition.x += areaOffset.x * facingDir;
        spawnPosition.y += areaOffset.y;

        // �����ش�ʵ��
        activeSpike = Instantiate(
            spikePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // ���÷���
        Vector3 scale = activeSpike.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDir;
        activeSpike.transform.localScale = scale;

        // ��ʼ�˺����Э��
        StartCoroutine(DamageSequence());

        // ����ش�
        StartCoroutine(CleanupSpike());
    }

    // �˺�����
    private IEnumerator DamageSequence()
    {
        // �ȴ��˺�����ʱ��
        yield return new WaitForSeconds(damageDelay);

        // ִ���˺����
        DetectAndAffectEnemies();
    }

    // ��ⲢӰ�����
    private void DetectAndAffectEnemies()
    {
        if (player == null) return;

        // ��ȡ��ҳ���
        int facingDir = player.getFacingDir();

        // �������������ĵ�
        Vector2 center = (Vector2)player.transform.position;
        center.x += areaOffset.x * facingDir;
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
                Vector2 knockbackDirection = new Vector2(facingDir, 0);
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
            }
        }
    }

    // ����ش�
    private IEnumerator CleanupSpike()
    {
        // �ȴ���������
        yield return new WaitForSeconds(animationDuration - damageDelay);

        if (activeSpike != null)
        {
            Destroy(activeSpike);
            activeSpike = null;
        }

        player.SetCastingSkill(false);
        isCasting = false;
    }

    // �����жϴ���
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

    // ���ӻ�����
    void OnDrawGizmosSelected()
    {
        if (!showGizmo || player == null) return;

        // ��ȡ��ҳ���
        int facingDir = player.getFacingDir();

        // �������������ĵ�
        Vector3 center = player.transform.position;
        center.x += areaOffset.x * facingDir;
        center.y += areaOffset.y;

        // ���Ƽ������
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawCube(center, new Vector3(areaSize.x, areaSize.y, 0.1f));

        // ���Ʒ���ָʾ
        Gizmos.color = Color.red;
        Vector3 start = center;
        Vector3 end = start + Vector3.right * areaSize.x * 0.5f * facingDir;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}