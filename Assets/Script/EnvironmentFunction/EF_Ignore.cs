using UnityEngine;
using System.Collections;

public class EF_Ignore : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("��Ҷ����Tag����")]
    public string playerTag = "Player";

    [Tooltip("��ǽЧ������ʱ��(��)")]
    [Range(1f, 10f)] public float ignoreTime = 3f;

    [Tooltip("��ǽʱ�İ�͸����ɫ")]
    public Color fadeColor = new Color(1, 1, 1, 0.5f);

    [Header("�������")]
    [Tooltip("ǽ�������ײ�壨��Trigger��")]
    public Collider2D wallCollider;

    [Tooltip("ǽ�����Ⱦ��")]
    public Renderer wallRenderer;

    private Color _originalColor;
    private bool _isActive = false;

    void Start()
    {
        // �Զ���ȡ����������
        if (wallCollider == null)
            wallCollider = GetComponentInParent<Collider2D>();

        if (wallRenderer == null)
            wallRenderer = GetComponentInParent<Renderer>();

        if (wallRenderer != null)
            _originalColor = wallRenderer.material.color;
    }

    // �˽ű����ڴ������Ӷ�����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive && other.CompareTag(playerTag))
        {
            StartCoroutine(ActivateEffect());
            Debug.Log("��ҽ���ǽ��");
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;

        // ����ǽ����ײ������ǽ��
        if (wallCollider != null)
            wallCollider.enabled = false;

        // �ı�ǽ���Ӿ�Ч��
        if (wallRenderer != null)
            wallRenderer.material.color = fadeColor;

        yield return new WaitForSeconds(ignoreTime);

        // �ָ�ǽ����ײ
        if (wallCollider != null)
            wallCollider.enabled = true;

        // �ָ��Ӿ�
        if (wallRenderer != null)
            wallRenderer.material.color = _originalColor;

        _isActive = false;
    }
}