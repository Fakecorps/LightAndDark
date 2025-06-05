using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltField : MonoBehaviour
{
    [Header("Field Settings")]
    public float maxRadius = 5f;          // 力场最大半径
    public float expandSpeed = 2f;        // 扩张速度
    public float duration = 5f;           // 力场持续时间

    [Header("Rendering Settings")]
    public string sortingLayer = "Foreground"; // 渲染层级
    public int sortingOrder = 10;          // 渲染优先级

    [Header("Detection Settings")]
    public LayerMask detectionLayers;     // 要检测的层级
    public float detectionInterval = 0.1f; // 检测间隔（秒）

    private float detectionTimer = 0f;
    private HashSet<Collider2D> affectedObjects = new HashSet<Collider2D>();


    private CircleCollider2D fieldCollider;
    private SpriteRenderer fieldRenderer;
    private float currentRadius = 0f;
    private float timer = 0f;

    void Start()
    {
        // 初始化组件
        fieldCollider = gameObject.AddComponent<CircleCollider2D>();
        fieldCollider.isTrigger = true;

        fieldRenderer = gameObject.AddComponent<SpriteRenderer>();
        fieldRenderer.sprite = CreateFieldSprite();
        fieldRenderer.color = Color.black;

        // 关键：设置渲染层级和优先级
        fieldRenderer.sortingLayerName = sortingLayer;
        fieldRenderer.sortingOrder = sortingOrder;

        // 初始缩放设置为0
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // 扩张阶段
        if (currentRadius < maxRadius)
        {
            currentRadius = Mathf.Min(currentRadius + expandSpeed * Time.deltaTime, maxRadius);
            UpdateFieldSize();
        }

        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            DetectColliders();
        }
        // 持续阶段计时
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateFieldSize()
    {
        // 更新碰撞体和渲染器大小
        float scale = currentRadius / 0.5f;  // 0.5是基础精灵的原始半径
        transform.localScale = new Vector3(scale, scale, 1f);
        fieldCollider.radius = 0.5f;  // 保持碰撞体在基础大小，缩放由transform控制
    }

    private Sprite CreateFieldSprite()
    {
        // 动态创建圆形精灵
        int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        Color[] pixels = new Color[textureSize * textureSize];
        float center = textureSize / 2f;
        float radius = textureSize / 4f;  // 原始半径为纹理的1/4

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                Color pixelColor = distance <= radius ? Color.black : Color.clear;
                pixels[y * textureSize + x] = pixelColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize / (radius * 2)  // 每单位像素数
        );
    }

    // 触发检测
    private void DetectColliders()
    {
        // 检测当前半径内的所有碰撞体
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            currentRadius,
            detectionLayers
        );

        // 处理新进入的碰撞体
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy_Goblin>(out var goblin))
            {
                Debug.Log("Goblin detected!");
            }
        }
    }
    private void OnEnterField(Collider2D other)
    {

    }

    private void OnExitField(Collider2D other)
    {

    }
}
