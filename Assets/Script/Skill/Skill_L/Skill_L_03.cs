using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_03 : Skill
{
    public static Skill_L_03 Instance;

    [Header("Skill Settings")]
    public int damage = 60;                   // �����˺�
    public float stunDuration = 1.5f;            // ѣ��ʱ��
    public float knockbackForce = 10f;           // ��������
    public float skillRange = 5f;                // �������÷�Χ
    public float width = 2f;                     // ���ܿ��
    public float spikeRiseDuration = 0.5f;       // �ش��������ʱ��
    public LayerMask enemyLayer;

    [Header("Prefabs")]
    public GameObject spikePrefab;               // �ش�Ԥ����

    private List<GameObject> activeSpikes = new List<GameObject>(); // ���ɵĵش�
    private bool isCasting;                      // �Ƿ�����ʩ��

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

        // ��ӵ�����Ϣ
        Debug.Log("Skill_L_03 activated");
    }

    // �����¼����ã������ش�
    public void CreateSpikes()
    {
        if (player == null || spikePrefab == null)
            return;

        // ������������
        Vector3 playerPos = player.transform.position;
        Vector3 playerForward = player.transform.forward;

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
                Vector3 spikePos = playerPos + playerForward * rowDistance;
                spikePos += player.transform.right * colOffset;

                // �ش̳�ʼλ���ڵ�������
                spikePos.y -= 2f;

                // �����ش�
                GameObject spike = Instantiate(
                    spikePrefab,
                    spikePos,
                    Quaternion.LookRotation(playerForward)
                );
                activeSpikes.Add(spike);

                // �������𶯻�
                StartCoroutine(RiseSpike(spike, spikePos + Vector3.up * 2f));
            }
        }

        // ��ⲢӰ�����
        DetectAndAffectEnemies();

        // ����ش�
        StartCoroutine(CleanupSpikes());
    }

    // �ش����𶯻�
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

    // ��ⲢӰ�����
    private void DetectAndAffectEnemies()
    {
        // ʹ�ú������������
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
                // ����˺�
                enemy.TakeDamage(damage);

                // Ӧ��ѣ��
                enemy.ApplyStun(stunDuration);

                // Ӧ�û��ˣ�����Ϊ���ǰ����
                Vector3 knockbackDirection = player.transform.forward;
                enemy.ApplyKnockback(knockbackDirection * knockbackForce);
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
        }
    }

    // ���ӻ�����
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        // ���Ƽ�����������
        Vector3 center = player.transform.position + player.transform.forward * (skillRange / 2f);
        Vector3 size = new Vector3(width, 4f, skillRange);

        // Ӧ�������ת
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
