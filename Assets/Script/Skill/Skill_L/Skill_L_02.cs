using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // ���������
    [Range(1f, 20f)] public float chainRange = 8f; // �ɻ������������
    [Range(1f, 10f)] public float pullSpeed = 5f;  // �ɻ�����������ȡ�ٶ�
    public float stunDuration = 1f;      // ѣ��ʱ��
    [Range(0.5f, 5f)] public float minDistance = 1.5f; // �ɻ�����������С����

    [Header("Visual Settings")]
    public GameObject chainVisualPrefab; // �����������ͼԤ����
    [Range(1f, 20f)] public float chainExtendSpeed = 10f; // �ɻ��������������ٶ�
    [Range(1f, 20f)] public float chainRetractSpeed = 15f; // �ɻ����������ջ��ٶ�
    [Range(0.1f, 2f)] public float chainWidth = 0.5f; // �ɻ����������������
    [Range(0f, 1f)] public float chainTipOffset = 0.1f;  // �ɻ��������ļ��ƫ����

    [Header("Visualization Tools")]
    [Tooltip("�ڱ༭������ʾ���ӻ�����")]
    public bool showVisualization = true;
    [Range(0f, 1f)] public float previewLength = 1f; // Ԥ�����ȱ���

    [Header("Detection Settings")]
    public LayerMask enemyLayer;         // ���˲㼶
    public LayerMask groundLayer;        // ����㼶

    private Enemy capturedEnemy;         // ������ĵ���
    private bool isPulling;              // �Ƿ�������ȡ����
    public static Skill_L_02 skill_L_02;

    private int facingDir;               // ��ҳ���
    private float enemyOriginalY;        // ����ԭʼ�߶�

    // �����Ӿ����
    private GameObject chainVisual;      // �����Ӿ�����
    private Transform chainTip;          // �������
    private bool isExtending = false;    // �����Ƿ���������
    private bool isRetracting = false;   // �����Ƿ������ջ�
    private Vector3 chainTarget;         // ����Ŀ��λ��
    private Vector3 chainOrigin;         // ������ʼλ�ã��̶���
    private float chainProgress;         // ������ǰ����

    // ʩ��״̬
    private bool isCasting = false;      // �Ƿ�����ʩ��

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // �������ʩ��״̬
        player.SetCastingSkill(true);
        isCasting = true;

        // ������ҳ���
        facingDir = player.getFacingDir();

        // ���ù̶���ʼ��
        chainOrigin = castPoint.position;

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
                if (capturedEnemy != null && capturedEnemy.IsAlive())
                {
                    // �������ԭʼ�߶�
                    enemyOriginalY = capturedEnemy.transform.position.y;

                    // Ӧ��ѣ��Ч��
                    capturedEnemy.ApplyStun(stunDuration);

                    // ��������Ŀ��λ�ã��ϸ�ˮƽ����
                    chainTarget = new Vector3(
                        capturedEnemy.transform.position.x,
                        chainOrigin.y, // ʹ���뷢�����ͬ�ĸ߶�
                        capturedEnemy.transform.position.z
                    );

                    // ���������Ӿ�
                    CreateChainVisual();

                    // ��ʼ��������
                    StartCoroutine(ExtendChain());

                    // ��ʼ��ȡЭ��
                    StartCoroutine(PullEnemy());
                }
            }
        }
        else
        {
            // û�����е��ˣ��������쵽�����루�ϸ�ˮƽ����
            chainTarget = new Vector3(
                castPoint.position.x + facingDir * chainRange,
                castPoint.position.y, // �뷢�����ͬ�߶�
                castPoint.position.z
            );

            // ���������Ӿ�
            CreateChainVisual();

            // ��ʼ����Ȼ���ջ�
            StartCoroutine(ExtendThenRetract());
        }
    }

    // ���������Ӿ� - ʹ�������ṩ����ͼ
    private void CreateChainVisual()
    {
        if (chainVisualPrefab == null)
        {
            Debug.LogWarning("�����Ӿ�Ԥ����δ����");
            return;
        }

        // ������������
        if (chainVisual != null)
        {
            Destroy(chainVisual);
        }

        // ��������������
        chainVisual = Instantiate(
            chainVisualPrefab,
            chainOrigin, // �̶��ڷ����
            Quaternion.identity
        );

        // ��ȡ�������
        chainTip = chainVisual.transform.Find("Tip");
        if (chainTip == null && chainVisual.transform.childCount > 0)
        {
            chainTip = chainVisual.transform.GetChild(0);
        }

        // ��ʼ����
        chainVisual.transform.localScale = new Vector3(0.1f, chainWidth, 1f);
        chainProgress = 0f;

        // ���ó�ʼλ�ú���ת
        UpdateChainVisual();
    }

    // ���������Ӿ� - β�˹̶���ǰ������
    private void UpdateChainVisual()
    {
        if (chainVisual == null) return;

        // ���㷽��
        Vector3 direction = (chainTarget - chainOrigin).normalized;

        // ʼ�չ̶��ڷ����
        chainVisual.transform.position = chainOrigin;

        // ������ת������Ŀ�꣩
        chainVisual.transform.right = direction;

        // ���㵱ǰ����
        float currentLength = Vector3.Distance(chainOrigin, chainTarget) * chainProgress;

        // �������ţ����ȺͿ�ȣ�
        chainVisual.transform.localScale = new Vector3(
            currentLength, // X�����ſ��Ƴ���
            chainWidth,    // Y�����ſ��ƿ��
            1f
        );

        // �������λ��
        if (chainTip != null)
        {
            // ���λ�� = ��� + ���� * ��ǰ����
            chainTip.position = chainOrigin + direction * currentLength;
        }
    }

    // ��������
    private IEnumerator ExtendChain()
    {
        isExtending = true;
        isRetracting = false;

        float totalDistance = Vector3.Distance(chainOrigin, chainTarget);
        float currentDistance = 0f;

        while (currentDistance < totalDistance && isExtending)
        {
            currentDistance += chainExtendSpeed * Time.deltaTime;
            chainProgress = Mathf.Clamp01(currentDistance / totalDistance);

            UpdateChainVisual();
            yield return null;
        }

        chainProgress = 1f;
        UpdateChainVisual();
        isExtending = false;
    }

    // ����Ȼ���ջأ�����δ���е��ˣ�
    private IEnumerator ExtendThenRetract()
    {
        yield return StartCoroutine(ExtendChain());
        yield return new WaitForSeconds(0.2f); // ����ͣ��
        yield return StartCoroutine(RetractChain());
    }

    // �ջ����� - ��Ŀ���������ջ�
    private IEnumerator RetractChain()
    {
        isRetracting = true;
        isExtending = false;

        float totalDistance = Vector3.Distance(chainOrigin, chainTarget);
        float currentDistance = totalDistance;

        while (currentDistance > 0 && isRetracting)
        {
            currentDistance -= chainRetractSpeed * Time.deltaTime;
            chainProgress = Mathf.Clamp01(currentDistance / totalDistance);

            UpdateChainVisual();
            yield return null;
        }

        // ��������
        if (chainVisual != null)
        {
            Destroy(chainVisual);
            chainVisual = null;
            chainTip = null;
        }

        // ����ʩ��״̬
        EndSkillCasting();
        isRetracting = false;
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

        // ��ȡѭ�� - �ϸ�ˮƽ�ƶ�
        while (isPulling && capturedEnemy != null && capturedEnemy.IsAlive())
        {
            // ������˵���ҵ�ˮƽ����
            Vector3 playerPos = player.transform.position;
            Vector3 enemyPos = capturedEnemy.transform.position;

            // �ϸ�ˮƽ������㣨����Y�ᣩ
            float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);

            // �������С����Сֵ��ֹͣ��ȡ
            if (horizontalDistance <= minDistance)
            {
                break;
            }

            // ������ȡ���򣨽�ˮƽ����
            float pullDirection = Mathf.Sign(playerPos.x - enemyPos.x);

            // �����ƶ���������ˮƽ�ƶ���
            float moveAmount = pullDirection * pullSpeed * Time.deltaTime;

            // �ƶ����ˣ����ı�X���꣩
            Vector3 newPosition = new Vector3(
                enemyPos.x + moveAmount,
                enemyOriginalY, // �ϸ񱣳�ԭʼ�߶�
                enemyPos.z
            );

            capturedEnemy.transform.position = newPosition;

            // ��������Ŀ��λ�ã��ϸ�ˮƽ���򣬱����뷢�����ͬ�߶ȣ�
            chainTarget = new Vector3(
                newPosition.x,
                chainOrigin.y, // ʹ���뷢�����ͬ�ĸ߶�
                newPosition.z
            );

            // ���������Ӿ�
            UpdateChainVisual();

            yield return null;
        }

        // �ָ���ײ״̬
        if (enemyCollider != null)
        {
            enemyCollider.enabled = originalColliderEnabled;
        }

        // ��ʼ�ջ�����
        StartCoroutine(RetractChain());

        // ������ȡ
        capturedEnemy = null;
        isPulling = false;
    }

    // �ж���ȡ
    public void CancelPull()
    {
        isPulling = false;
        isExtending = false;

        // ��ʼ�ջ�����
        if (chainVisual != null)
        {
            StartCoroutine(RetractChain());
        }
        else
        {
            EndSkillCasting();
        }

        if (capturedEnemy != null)
        {
            // �ָ���ײ״̬
            Collider2D enemyCollider = capturedEnemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }

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
        if (isCasting)
        {
            StopAllCoroutines();
            CancelPull();
            EndSkillCasting();
        }
    }

    // ���ӻ���������
    private void OnDrawGizmosSelected()
    {
        if (!showVisualization || castPoint == null) return;

        int facing = Application.isPlaying ? facingDir : 1;
        if (facing == 0) facing = 1; // ����0����

        // ��������������
        Gizmos.color = new Color(0, 0.5f, 1f, 0.7f); // ��͸����ɫ
        Vector3 endPoint = new Vector3(
            castPoint.position.x + facing * chainRange,
            castPoint.position.y,
            castPoint.position.z
        );
        Gizmos.DrawLine(castPoint.position, endPoint);

        // ������ȡֹͣ����
        Gizmos.color = new Color(1f, 1f, 0, 0.5f); // ��͸����ɫ
        Gizmos.DrawWireSphere(castPoint.position, minDistance);

        // ����Ԥ������
        if (previewLength > 0)
        {
            Vector3 previewTarget = new Vector3(
                castPoint.position.x + facing * chainRange * previewLength,
                castPoint.position.y,
                castPoint.position.z
            );

            // ���㷽��
            Vector3 direction = (previewTarget - castPoint.position).normalized;
            float previewDistance = Vector3.Distance(castPoint.position, previewTarget);

            // ����Ԥ������
            Gizmos.color = new Color(0, 1f, 0, 0.8f); // ��͸����ɫ
            Gizmos.DrawLine(castPoint.position, previewTarget);

            // ����Ԥ�����
            Gizmos.DrawWireSphere(previewTarget, 0.1f);

            // ����Ԥ�����ָʾ��
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            Gizmos.DrawLine(
                previewTarget - perpendicular * chainWidth * 0.5f,
                previewTarget + perpendicular * chainWidth * 0.5f
            );
        }
    }

    // ����ʩ��״̬
    private void EndSkillCasting()
    {
        if (isCasting && player != null)
        {
            player.SetCastingSkill(false);
            isCasting = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Skill_L_02))]
public class Skill_L_02Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Skill_L_02 skill = (Skill_L_02)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("���ӻ�Ԥ������", EditorStyles.boldLabel);

        // ���Ԥ������
        skill.previewLength = EditorGUILayout.Slider("��������Ԥ��", skill.previewLength, 0f, 1f);

        // ��ӿ��ӻ�����
        skill.showVisualization = EditorGUILayout.Toggle("��ʾ���ӻ�", skill.showVisualization);

        // ���ˢ�°�ť
        if (GUILayout.Button("ˢ��Ԥ��"))
        {
            // ǿ���ػ泡����ͼ
            SceneView.RepaintAll();
        }

        EditorGUILayout.HelpBox("����������ʹ��'ˢ��Ԥ��'��ť���³�����ͼ�еĿ��ӻ�Ч����", MessageType.Info);
    }
}
#endif