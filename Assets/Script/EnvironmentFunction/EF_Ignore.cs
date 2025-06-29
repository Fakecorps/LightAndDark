using UnityEngine;
using System.Collections;

public class EF_Ignore : MonoBehaviour
{
    [Header("基本设置")]
    [Tooltip("玩家对象的Tag名称")]
    public string playerTag = "Player2";

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

        if (wallRenderer != null)
        {
            // 创建材质实例避免共享问题
            wallRenderer.material = new Material(wallRenderer.material);
            _originalColor = wallRenderer.material.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive && other.CompareTag(playerTag))
        {
            StartCoroutine(ActivateEffect());
            Debug.Log("玩家进入墙体触发区域");
        }
    }

    IEnumerator ActivateEffect()
    {
        _isActive = true;
        Debug.Log("穿墙效果启动");

        // 禁用墙体碰撞
        if (wallCollider != null)
        {
            Debug.Log($"禁用碰撞体前状态: {wallCollider.enabled}");
            wallCollider.enabled = false;

            // 强制物理系统立即更新
            Physics2D.SyncTransforms();

            Debug.Log($"禁用碰撞体后状态: {wallCollider.enabled}");
        }

        // 改变墙体视觉效果
        if (wallRenderer != null)
        {
            wallRenderer.material.color = fadeColor;
            Debug.Log("设置半透明颜色");
        }

        yield return new WaitForSeconds(ignoreTime);
        Debug.Log($"效果结束，恢复状态");

        // 恢复墙体碰撞
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
            Physics2D.SyncTransforms();
            Debug.Log("碰撞体已启用");
        }

        // 恢复视觉
        if (wallRenderer != null)
        {
            wallRenderer.material.color = _originalColor;
            Debug.Log("恢复原始颜色");
        }

        _isActive = false;
    }
}