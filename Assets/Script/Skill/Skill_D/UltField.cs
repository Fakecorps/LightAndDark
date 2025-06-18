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

    [Header("Debug Visualization")]
    public bool showColliderOutline = true; // 可随时在Inspector中开关
    public Color outlineColor = Color.red;   // 自定义轮廓颜色
    public int outlineResolution = 50;

    [Header("Field Sprite Settings")]
    public float spriteBaseRadius = 0.5f;


    private CircleCollider2D fieldCollider;
    private SpriteRenderer fieldRenderer;
    private LineRenderer fieldOutline;
    private float currentRadius = 0f;
    private float timer = 0f;

    public List<Enemy> controlledEnemies = new List<Enemy>();
    private bool isActive = true;

    [Header("Phantom Settings")]
    public GameObject phantomPrefab; // 幻影预制体
    public float phantomDuration = 0.5f; // 幻影持续时间
    public int phantomDamage = 10; // 幻影攻击伤害


    void Start()
    {
        // 初始化组件
        fieldCollider = gameObject.AddComponent<CircleCollider2D>();
        fieldCollider.isTrigger = true;
        fieldCollider.radius = 0f; // 初始半径设为0

        fieldRenderer = gameObject.AddComponent<SpriteRenderer>();

        // 使用新的精灵创建方法
        fieldRenderer.sprite = CreateFieldSprite(spriteBaseRadius);
        fieldRenderer.color = Color.black;

        // 设置渲染层级
        fieldRenderer.sortingLayerName = sortingLayer;
        fieldRenderer.sortingOrder = sortingOrder;

        // 初始缩放设置为0
        transform.localScale = Vector3.zero;
    }

    private void InitializeOutlineRenderer()
    {
        if (fieldOutline != null) return;

        fieldOutline = gameObject.AddComponent<LineRenderer>();
        fieldOutline.useWorldSpace = true; // 使用世界空间
        fieldOutline.loop = true;
        fieldOutline.positionCount = outlineResolution + 1;

        fieldOutline.material = new Material(Shader.Find("Sprites/Default"));
        fieldOutline.startColor = outlineColor;
        fieldOutline.endColor = outlineColor;
        fieldOutline.startWidth = 0.1f;
        fieldOutline.endWidth = 0.1f;

        fieldOutline.sortingLayerName = sortingLayer;
        fieldOutline.sortingOrder = sortingOrder + 1;
    }

    void Update()
    {
        controlledEnemies.RemoveAll(enemy =>
          enemy == null || !enemy.IsAlive());
        // 在扩张前初始化轮廓
        if (showColliderOutline && fieldOutline == null)
        {
            InitializeOutlineRenderer();
        }
        detectionTimer += Time.deltaTime;
        timer += Time.deltaTime;
        // 扩张阶段
        if (currentRadius < maxRadius)
        {
            currentRadius = Mathf.Min(currentRadius + expandSpeed * Time.deltaTime, maxRadius);
            UpdateFieldSize();
        }

        // ... 其他代码保持不变 ...

        if (showColliderOutline && fieldOutline != null)
        {
            UpdateOutline();
        }
        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            DetectColliders();
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateFieldSize()
    {
        // 更新碰撞体半径
        fieldCollider.radius = currentRadius;

        // 防止除以零错误
        if (spriteBaseRadius <= 0.01f) spriteBaseRadius = 0.5f;

        // 使用精确的缩放计算
        float scale = currentRadius / spriteBaseRadius;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void UpdateOutline()
    {
        // 直接使用当前半径（世界空间）
        float radius = currentRadius;
        float anglePerStep = 360f / outlineResolution;

        // 确保轮廓在正确的位置
        for (int i = 0; i <= outlineResolution; i++)
        {
            float angle = i * anglePerStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // 使用世界空间坐标
            Vector3 worldPos = transform.position + new Vector3(x, y, 0);
            fieldOutline.SetPosition(i, worldPos);
        }
    }
    private Sprite CreateFieldSprite(float baseRadius)
    {
        int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize);

        // 直接使用传入的baseRadius
        float pixelsPerUnit = textureSize / (2f * spriteBaseRadius);
        float center = textureSize / 2f;
        float radiusInPixels = spriteBaseRadius * pixelsPerUnit;

        Color[] pixels = new Color[textureSize * textureSize];
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                Color pixelColor = distance <= radiusInPixels ? Color.black : Color.clear;
                pixels[y * textureSize + x] = pixelColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // 使用计算出的pixelsPerUnit
        return Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );
    }

    // 触发检测
    private void DetectColliders()
    {
        float actualRadius = fieldCollider.radius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
         transform.position,
         actualRadius,  // 使用当前半径
         detectionLayers
     );
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out var enemy))
            {
                if (!controlledEnemies.Contains(enemy))
                {
                    OnEnterField(enemy);
                }
            }
        }
    }

    private void OnEnterField(Enemy enemy)
    {
        if (!enemy.IsAlive()) return;
        if (!controlledEnemies.Contains(enemy))
        {
            controlledEnemies.Add(enemy);
            enemy.SetControlledState(true);
        }
    }

    private void OnExitField(Enemy enemy)
    {
        if (!enemy.IsAlive()) return;

        if (controlledEnemies.Contains(enemy))
        {
            controlledEnemies.Remove(enemy);
            enemy.SetControlledState(false);
        }
    }

    public void TriggerPhantomAttack()
    {
        controlledEnemies.RemoveAll(enemy =>
       enemy == null || !enemy.IsAlive());

        if (!isActive || controlledEnemies.Count == 0) return;

        // 随机选择一个敌人
        int randomIndex = Random.Range(0, controlledEnemies.Count);
        Enemy target = controlledEnemies[randomIndex];

        if (target != null)
        {
            CreatePhantom(target);
        }
        else
        {
            controlledEnemies.RemoveAt(randomIndex); // 移除无效敌人
        }
    }

    private void CreatePhantom(Enemy target)
    {
        // 在敌人面前生成幻影
        Vector3 spawnPos = target.transform.position +
                          new Vector3(target.getFacingDir() * 1.5f, 0.5f, 0);

        GameObject phantom = Instantiate(
            phantomPrefab,
            spawnPos,
            Quaternion.identity
        );

        PhantomController controller = phantom.GetComponent<PhantomController>();
        if (controller == null)
        {
            controller = phantom.AddComponent<PhantomController>();
        }

        controller.Initialize(target, phantomDamage, phantomDuration);
    }
    void OnDestroy()
    {
        isActive = false;
        // 力场结束时释放所有敌人
        foreach (var enemy in controlledEnemies)
        {
            if (enemy != null && enemy.IsAlive())
            {
                enemy.SetControlledState(false);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 绘制碰撞体范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fieldCollider.radius);

        // 绘制视觉范围
        Gizmos.color = Color.blue;
        float visualRadius = spriteBaseRadius * transform.localScale.x;
        Gizmos.DrawWireSphere(transform.position, visualRadius);
    }


}
