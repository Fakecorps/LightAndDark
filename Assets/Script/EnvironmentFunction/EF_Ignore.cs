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

    private Collider _collider;
    private Renderer _renderer;
    private Color _originalColor;
    private bool _isActive = false;

    void Start()
    {
        // ��ȡ��Ҫ���
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ����Ƿ������Ч��δ����
        if (!_isActive && other.CompareTag(playerTag))
        {
            // ������ǽЧ��
            StartCoroutine(ActivateEffect());
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;

        // ������ײ��
        if (_collider != null) _collider.enabled = false;

        // �ı��Ӿ�Ч��
        if (_renderer != null)
        {
            _renderer.material.color = fadeColor;
        }

        // �ȴ�����ʱ��
        yield return new WaitForSeconds(ignoreTime);

        // �ָ���ײ��
        if (_collider != null) _collider.enabled = true;

        // �ָ��Ӿ�
        if (_renderer != null) _renderer.material.color = _originalColor;

        _isActive = false;
    }
}