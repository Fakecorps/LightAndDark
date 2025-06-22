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

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // ���˲㼶
    public LayerMask groundLayer;        // ����㼶

    private Enemy capturedEnemy;         // ������ĵ���
    private bool isPulling;              // �Ƿ�������ȡ����
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // ��ҳ���
    private float enemyOriginalY;        // ����ԭʼ�߶�

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

                    // ��ʼ��ȡЭ��
                    StartCoroutine(PullEnemy());
                }
            }
        }
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

            // ���Ƶ����ߣ�ʵ��ʹ��ʱ���滻Ϊ�Ӿ�Ч����
            Debug.DrawLine(
                castPoint.position,
                capturedEnemy.transform.position,
                Color.yellow,
                0.1f
            );

            yield return null;
        }

        // �ָ���ײ״̬
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // ȷ�������ڵ�����
        PlaceEnemyOnGround();

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
        if (isPulling)
        {
            StopAllCoroutines();
            player.SetCastingSkill(false);
            isPulling = false;
            CancelPull();
        }
    }
}