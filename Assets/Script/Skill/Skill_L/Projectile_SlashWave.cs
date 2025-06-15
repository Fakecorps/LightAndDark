using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_SlashWave : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float maxDistance = 15f;

    [Header("Damage Settings")]
    public int damagePerTick = 5;
    public float tickInterval = 0.3f;
    public float knockbackForce = 2f;
    public LayerMask enemyLayer;

    [Header("Collider Settings")]
    public float colliderScaleFactor = 0.8f; // 碰撞体缩放因子，默认0.8

    private Vector3 startPosition;
    private float nextDamageTime;
    private List<Enemy> affectedEnemies = new List<Enemy>();
    private int direction = 1;
    private bool isDestroying = false;

    public void SetDirection(int dir)
    {
        direction = dir;

        // 根据方向调整精灵方向
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Vector3 scale = sr.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir == 1 ? 1 : -1);
            sr.transform.localScale = scale;
        }
    }

    // 设置碰撞体大小（考虑缩放因子）
    public void SetColliderSize(float width, float height)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // 应用缩放因子减小碰撞体
            collider.size = new Vector2(
                width * colliderScaleFactor,
                height * colliderScaleFactor
            );
        }
    }

    private void Start()
    {
        startPosition = transform.position;
        nextDamageTime = Time.time + tickInterval;
        DetectInitialEnemies();
    }

    private void Update()
    {
        if (isDestroying) return;

        // 移动
        Vector3 moveDirection = Vector3.right * direction;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 检查距离限制
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            DestroyWave();
            return;
        }

        // 处理伤害
        if (Time.time >= nextDamageTime)
        {
            ApplyDamageAndKnockback();
            nextDamageTime = Time.time + tickInterval;
        }
    }

    private void DetectInitialEnemies()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;

        // 使用实际碰撞体大小进行检测
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position,
            collider.size,
            0,
            enemyLayer
        );

        foreach (Collider2D col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
            }
        }
    }
    private void ApplyDamageAndKnockback()
    {
        // 创建临时列表处理有效敌人
        List<Enemy> validEnemies = new List<Enemy>();

        foreach (Enemy enemy in affectedEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                validEnemies.Add(enemy);
            }
        }

        // 更新受影响敌人列表
        affectedEnemies = validEnemies;

        // 对有效敌人造成伤害
        foreach (Enemy enemy in affectedEnemies)
        {
            enemy.TakeDamage(damagePerTick);
            Vector2 knockbackDirection = new Vector2(direction, 0);
            enemy.ApplyKnockback(knockbackDirection * knockbackForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Remove(enemy);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Obstacle")) != 0)
        {
            DestroyWave();
        }
    }

    private void DestroyWave()
    {
        if (isDestroying) return;
        isDestroying = true;

        // 停止所有击退效果
        foreach (Enemy enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.StopKnockback();
            }
        }

        Destroy(gameObject);
    }
}