using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // ���������
    public GameObject chainPrefab;       // ����Ԥ����
    public LayerMask enemyLayer;         // ���˲㼶
    public float chainRange = 8f;        // �������
    public float pullForce = 35f;        // ��ȡ����
    public float stunDuration = 1f;      // ѣ��ʱ��
    public float chainWidth = 0.3f;      // �������

    private GameObject activeChain;      // ��ǰ����Ĺ���
    private Transform capturedEnemy;     // ������ĵ���
    public static Skill_L_02 skill_L_02 ;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Debug.Log("Skill_L_02");

        if (player == null)
            player = Player.ActivePlayer;

        // ��������
        player.anim.SetTrigger("Skill_L_02");
    }

    // �����¼����ã��ڶ����ؼ�֡����¼���
    public void OnCastAnimationEvent()
    {
        Debug.Log("Cast");

        if (player == null) return;

        // ��������ʵ��
        activeChain = Instantiate(chainPrefab, castPoint.position, castPoint.rotation, castPoint);

        // ���߼�����
        RaycastHit hit;
        if (Physics.Raycast(castPoint.position, castPoint.forward, out hit, chainRange, enemyLayer))
        {
            capturedEnemy = hit.transform;
            Enemy enemy = capturedEnemy.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Ӧ��ѣ��Ч��
                enemy.ApplyStun(stunDuration);

                // ��ʼ��ȡЭ��
                StartCoroutine(PullEnemy(capturedEnemy));
            }

            // ���¹����Ӿ�
            UpdateChainVisual(hit.point);
        }
        else
        {
            // û�����е��ˣ��������쵽������
            Vector3 endPoint = castPoint.position + castPoint.forward * chainRange;
            UpdateChainVisual(endPoint);

            // ��������Э��
            StartCoroutine(RetractChain());
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    private void UpdateChainVisual(Vector3 endPoint)
    {
        if (!activeChain) return;

        LineRenderer line = activeChain.GetComponent<LineRenderer>();
        if (line == null) return;

        line.SetPosition(0, castPoint.position);
        line.SetPosition(1, endPoint);

        // ��̬�������
        line.widthCurve = new AnimationCurve(
            new Keyframe(0, chainWidth * 0.3f),
            new Keyframe(0.5f, chainWidth),
            new Keyframe(1, chainWidth * 0.3f)
        );
    }

    private IEnumerator PullEnemy(Transform enemy)
    {
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb == null) yield break;

        // Ŀ��λ��
        Vector3 targetPosition = player.transform.position + player.transform.forward * 10f;

        float pullDuration = 0.4f;
        float timer = 0;

        // ��ʱ������ײ���⿨ס
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider) enemyCollider.enabled = false;

        while (timer < pullDuration)
        {
            if (enemy == null) yield break;

            timer += Time.deltaTime;
            Vector3 direction = (targetPosition - enemy.position).normalized;
            enemyRb.velocity = direction * pullForce;

            // ���¹���ĩ��λ��
            UpdateChainVisual(enemy.position);

            yield return null;
        }

        // �ָ���ײ��
        if (enemyCollider) enemyCollider.enabled = true;

        // ���չ���
        StartCoroutine(RetractChain());
    }

    private IEnumerator RetractChain()
    {
        if (!activeChain) yield break;

        LineRenderer line = activeChain.GetComponent<LineRenderer>();
        Vector3 startPos = line.GetPosition(0);
        Vector3 endPos = line.GetPosition(1);

        float duration = 0.2f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            line.SetPosition(1, Vector3.Lerp(endPos, startPos, t));
            yield return null;
        }

        Destroy(activeChain);
        activeChain = null;
        capturedEnemy = null;
    }

    protected override void Start()
    {
        base.Start();
        skill_L_02 = this;
    }
}
