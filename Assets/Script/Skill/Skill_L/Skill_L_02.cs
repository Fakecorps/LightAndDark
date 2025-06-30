using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // ���������
    public float chainRange = 8f;        // �������
    public float pullSpeed = 5f;         // ��ȡ�ٶ�
    public float stunDuration = 1f;      // ѣ��ʱ��
    public float minDistance = 1.5f;     // ��ȡֹͣ����С����
    public float groundHeightOffset = 0.5f; // ����߶�ƫ��

    [Header("Visual Settings")]
    public GameObject chainSegmentPrefab; // ������Ԥ����
    public float segmentSize = 0.5f;     // ÿ�������εĳ���
    public float chainSpeed = 10f;       // ���������ٶ�
    public float chainWidth = 0.1f;      // �������
    public Color chainColor = Color.white; // ������ɫ

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // ���˲㼶
    public LayerMask groundLayer;        // ����㼶

    private Enemy capturedEnemy;         // ������ĵ���
    private bool isPulling;              // �Ƿ�������ȡ����
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // ��ҳ���
    private float enemyOriginalY;        // ����ԭʼ�߶�

    // �������
    private List<GameObject> chainSegments = new List<GameObject>();
    private bool isExtending = false;    // �����Ƿ���������
    private bool isRetracting = false;   // �����Ƿ������ջ�
    private Vector3 chainTarget;         // ����Ŀ��λ��

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // ������ҳ���
        facingDir = player.getFacingDir();

        // ��������
        player.anim.SetTrigger("Skill_L_02");
    }

    // �����¼����ã��ڶ����ؼ�֡����¼���
    public void OnCastAnimationEvent()
    {
        if (player == null) return;

        // ���ǰ����һ������
        RaycastHit2D hit = Physics2D.Raycast(
            castPoint.position,
            Vector2.right * facingDir,
            chainRange,
            enemyLayer
        );

        // ����Ƿ����е���
        if (hit.collider != null)
        {
            // ����Ƿ��ڵ��˲㼶
            if (enemyLayer == (enemyLayer | (1 << hit.collider.gameObject.layer)))
            {
                capturedEnemy = hit.collider.GetComponent<Enemy>();
                if (capturedEnemy != null)
                {
                    // �������ԭʼ�߶�
                    enemyOriginalY = capturedEnemy.transform.position.y;

                    // Ӧ��ѣ��Ч��
                    capturedEnemy.ApplyStun(stunDuration);

                    // ��������Ŀ��λ��
                    chainTarget = capturedEnemy.transform.position;

                    // ��ʼ��������
                    StartCoroutine(ExtendChain());

                    // ��ʼ��ȡЭ��
                    StartCoroutine(PullEnemy());
                }
            }
        }
        else
        {
            // û�����е��ˣ��������쵽������
            chainTarget = castPoint.position + Vector3.right * facingDir * chainRange;
            StartCoroutine(ExtendChain());
            StartCoroutine(RetractChain());
        }
    }

    // ��������
    private IEnumerator ExtendChain()
    {
        if (chainSegmentPrefab == null) yield break;

        isExtending = true;
        isRetracting = false;
        ClearChain();

        Vector3 startPos = castPoint.position;
        Vector3 direction = (chainTarget - startPos).normalized;
        float distance = Vector3.Distance(startPos, chainTarget);
        int segmentsNeeded = Mathf.CeilToInt(distance / segmentSize);

        float progress = 0f;

        while (progress < distance && isExtending)
        {
            progress += chainSpeed * Time.deltaTime;
            progress = Mathf.Min(progress, distance);

            Vector3 currentEnd = startPos + direction * progress;

            // ���»򴴽�������
            UpdateChainSegments(startPos, currentEnd);

            yield return null;
        }

        isExtending = false;
    }

    // ����������
    private void UpdateChainSegments(Vector3 start, Vector3 end)
    {
        // ������������
        float distance = Vector3.Distance(start, end);
        int segmentsNeeded = Mathf.CeilToInt(distance / segmentSize);
        Vector3 direction = (end - start).normalized;

        // ȷ�����㹻��������
        while (chainSegments.Count < segmentsNeeded)
        {
            GameObject segment = Instantiate(chainSegmentPrefab, castPoint.position, Quaternion.identity);
            chainSegments.Add(segment);

            // �����������
            SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = chainColor;
                renderer.sortingOrder = 5; // ȷ���ڽ�ɫǰ��
            }
        }

        // �Ƴ������
        while (chainSegments.Count > segmentsNeeded)
        {
            GameObject segment = chainSegments[chainSegments.Count - 1];
            chainSegments.RemoveAt(chainSegments.Count - 1);
            Destroy(segment);
        }

        // ��λ������
        for (int i = 0; i < segmentsNeeded; i++)
        {
            float segmentProgress = (i + 0.5f) * segmentSize;
            if (segmentProgress > distance) segmentProgress = distance - 0.1f;

            Vector3 segmentPos = start + direction * segmentProgress;

            // ����λ�ú���ת
            chainSegments[i].transform.position = segmentPos;
            chainSegments[i].transform.right = direction;

            // ��������
            chainSegments[i].transform.localScale = new Vector3(
                segmentSize,
                chainWidth,
                1f
            );
        }
    }

    // �ջ�����
    private IEnumerator RetractChain()
    {
        isRetracting = true;
        isExtending = false;

        float progress = Vector3.Distance(castPoint.position, chainTarget);

        while (progress > 0 && isRetracting)
        {
            progress -= chainSpeed * Time.deltaTime;
            progress = Mathf.Max(progress, 0);

            Vector3 currentEnd = castPoint.position + (chainTarget - castPoint.position).normalized * progress;

            // ����������
            UpdateChainSegments(castPoint.position, currentEnd);

            yield return null;
        }

        ClearChain();
        isRetracting = false;
    }

    // �������
    private void ClearChain()
    {
        foreach (GameObject segment in chainSegments)
        {
            Destroy(segment);
        }
        chainSegments.Clear();
    }

    private IEnumerator PullEnemy()
    {
        if (capturedEnemy == null) yield break;

        isPulling = true;

        // ����ԭʼ��ײ״̬
        Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
        bool originalColliderEnabled = enemyCollider != null ? enemyCollider.enabled : false;

        // ��ʱ������ײ���⿨ס
        if (enemyCollider != null) enemyCollider.enabled = false;

        // ��ȡѭ��
        while (isPulling && capturedEnemy != null)
        {
            // ������˵���ҵ�ˮƽ����
            Vector3 playerPos = player.transform.position;
            Vector3 enemyPos = capturedEnemy.transform.position;

            // ���ֵ���ԭʼ�߶�
            enemyPos.y = enemyOriginalY;

            // ����ˮƽ����
            float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);

            // �������С����Сֵ��ֹͣ��ȡ
            if (horizontalDistance <= minDistance)
            {
                break;
            }

            // ������ȡ���򣨽�ˮƽ����
            Vector3 pullDirection = new Vector3(
                Mathf.Sign(playerPos.x - enemyPos.x), // ˮƽ����
                0, // ��ֱ������Ϊ0�����ָ߶Ȳ���
                0
            );

            // �����ƶ���������ˮƽ�ƶ���
            Vector3 moveVector = pullDirection * pullSpeed * Time.deltaTime;

            // �ƶ����ˣ����ı�X���꣩
            capturedEnemy.transform.position = new Vector3(
                enemyPos.x + moveVector.x,
                enemyOriginalY, // ����ԭʼ�߶�
                enemyPos.z
            );

            // ��������Ŀ��λ��
            chainTarget = capturedEnemy.transform.position;

            yield return null;
        }

        // �ָ���ײ״̬
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // ȷ�������ڵ�����
        PlaceEnemyOnGround();

        // ��ʼ�ջ�����
        StartCoroutine(RetractChain());

        // ������ȡ
        capturedEnemy = null;
        isPulling = false;
    }

    // ȷ�����˷����ڵ�����
    private void PlaceEnemyOnGround()
    {
        if (capturedEnemy == null) return;

        RaycastHit2D groundHit = Physics2D.Raycast(
            capturedEnemy.transform.position,
            Vector2.down,
            5f,
            groundLayer
        );

        if (groundHit.collider != null)
        {
            Vector3 newPosition = capturedEnemy.transform.position;
            newPosition.y = groundHit.point.y + groundHeightOffset;
            capturedEnemy.transform.position = newPosition;
        }
    }

    // �ж���ȡ
    public void CancelPull()
    {
        isPulling = false;
        isExtending = false;

        // ��ʼ�ջ�����
        StartCoroutine(RetractChain());

        if (capturedEnemy != null)
        {
            // �ָ���ײ״̬
            Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }

            // ȷ�������ڵ�����
            PlaceEnemyOnGround();

            capturedEnemy = null;
        }
    }

    protected override void Update()
    {
        base.Update();

        // ������Ҫ�ڴ˸���λ�ã�ȫ����Э���д���
    }

    protected override void Start()
    {
        base.Start();
        skill_L_02 = this;

        // ��ȫ��ʼ��
        if (enemyLayer.value == 0)
        {
            Debug.LogWarning("enemyLayerδ���ã���ʹ��Ĭ�ϲ㼶");
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        // ���õ���㼶
        if (groundLayer.value == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
    }

    // �����ܱ�ȡ��ʱ����
    public void CancelSkill()
    {
        if (isPulling || isExtending)
        {
            StopAllCoroutines();
            player.SetCastingSkill(false);
            CancelPull();
        }
    }
}