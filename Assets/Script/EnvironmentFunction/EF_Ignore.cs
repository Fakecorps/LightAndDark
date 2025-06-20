using UnityEngine;
using System.Collections;

public class EF_Ignore : MonoBehaviour
{
    [Header("基本设置")]
    [Tooltip("玩家对象的Tag名称")]
    public string playerTag = "Player";

    [Tooltip("穿墙效果持续时间(秒)")]
    [Range(1f, 10f)] public float ignoreTime = 3f;

    [Tooltip("穿墙时的半透明颜色")]
    public Color fadeColor = new Color(1, 1, 1, 0.5f);

    private Collider _collider;
    private Renderer _renderer;
    private Color _originalColor;
    private bool _isActive = false;

    void Start()
    {
        // 获取必要组件
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否玩家且效果未激活
        if (!_isActive && other.CompareTag(playerTag))
        {
            // 启动穿墙效果
            StartCoroutine(ActivateEffect());
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;

        // 禁用碰撞器
        if (_collider != null) _collider.enabled = false;

        // 改变视觉效果
        if (_renderer != null)
        {
            _renderer.material.color = fadeColor;
        }

        // 等待持续时间
        yield return new WaitForSeconds(ignoreTime);

        // 恢复碰撞器
        if (_collider != null) _collider.enabled = true;

        // 恢复视觉
        if (_renderer != null) _renderer.material.color = _originalColor;

        _isActive = false;
    }
}