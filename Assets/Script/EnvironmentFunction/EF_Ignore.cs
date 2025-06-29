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

    [Header("组件引用")]
    [Tooltip("墙体的主碰撞体（非Trigger）")]
    public Collider2D wallCollider;

    [Tooltip("墙体的渲染器")]
    public Renderer wallRenderer;

    private Color _originalColor;
    private bool _isActive = false;

    void Start()
    {
        // 自动获取父对象的组件
        if (wallCollider == null)
            wallCollider = GetComponentInParent<Collider2D>();

        if (wallRenderer == null)
            wallRenderer = GetComponentInParent<Renderer>();

        if (wallRenderer != null)
            _originalColor = wallRenderer.material.color;
    }

    // 此脚本挂在触发器子对象上
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive && other.CompareTag(playerTag))
        {
            StartCoroutine(ActivateEffect());
            Debug.Log("玩家进入墙体");
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;

        // 禁用墙体碰撞（允许穿墙）
        if (wallCollider != null)
            wallCollider.enabled = false;

        // 改变墙体视觉效果
        if (wallRenderer != null)
            wallRenderer.material.color = fadeColor;

        yield return new WaitForSeconds(ignoreTime);

        // 恢复墙体碰撞
        if (wallCollider != null)
            wallCollider.enabled = true;

        // 恢复视觉
        if (wallRenderer != null)
            wallRenderer.material.color = _originalColor;

        _isActive = false;
    }
}