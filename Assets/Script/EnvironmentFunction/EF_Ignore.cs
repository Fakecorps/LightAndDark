using UnityEngine;
using System.Collections;

public class EF_Ignore : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("��Ҷ����Tag����")]
    public string playerTag = "Player2";

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

        if (wallRenderer != null)
        {
            // ��������ʵ�����⹲������
            wallRenderer.material = new Material(wallRenderer.material);
            _originalColor = wallRenderer.material.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive && other.CompareTag(playerTag))
        {
            StartCoroutine(ActivateEffect());
            Debug.Log("��ҽ���ǽ�崥������");
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;
        Debug.Log("��ǽЧ������");

        // ����ǽ����ײ
        if (wallCollider != null)
        {
            Debug.Log($"������ײ��ǰ״̬: {wallCollider.enabled}");
            wallCollider.enabled = false;

            // ǿ������ϵͳ��������
            Physics2D.SyncTransforms();

            Debug.Log($"������ײ���״̬: {wallCollider.enabled}");
        }

        // �ı�ǽ���Ӿ�Ч��
        if (wallRenderer != null)
        {
            wallRenderer.material.color = fadeColor;
            Debug.Log("���ð�͸����ɫ");
        }

        yield return new WaitForSeconds(ignoreTime);
        Debug.Log($"Ч���������ָ�״̬");

        // �ָ�ǽ����ײ
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
            Physics2D.SyncTransforms();
            Debug.Log("��ײ��������");
        }

        // �ָ��Ӿ�
        if (wallRenderer != null)
        {
            wallRenderer.material.color = _originalColor;
            Debug.Log("�ָ�ԭʼ��ɫ");
        }

        _isActive = false;
    }
}