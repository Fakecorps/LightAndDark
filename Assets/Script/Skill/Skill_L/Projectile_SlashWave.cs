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
    public float knockbackForce = 1f; // 增加击退力度
    public LayerMask enemyLayer;
    public float stunDuration = 0.5f;

    [Header("Visual Effects")]
    public GameObject hitEffectPrefab; // 受击特效
    public float hitEffectDuration = 0.2f;

    private Vector3 startPosition;
    private float nextDamageTime;
    private List<Enemy> affectedEnemies = new List<Enemy>();
    private int direction = 1;
    private bool isDestroying = false;
    private float knockbackMultiplier = 1.0f; // 击退倍率

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

    // 设置击退倍率（可选）
    public void SetKnockbackMultiplier(float multiplier)
    {
        knockbackMultiplier = multiplier;
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

        // 处理伤害和击退
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

        // 扩大检测范围以确保捕获敌人
        Vector2 detectionSize = collider.size * 1.2f;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position,
            detectionSize,
            0,
            enemyLayer
        );

        foreach (Collider2D col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                ApplyStunAndKnockback(enemy, true);
            }
        }
    }

    private void ApplyDamageAndKnockback()
    {
        for (int i = affectedEnemies.Count - 1; i >= 0; i--)
        {
            if (affectedEnemies[i] != null)
            {
                // 造成伤害
                affectedEnemies[i].TakeDamage(damagePerTick);

                // 应用击退和眩晕
                ApplyStunAndKnockback(affectedEnemies[i], false);
            }
            else
            {
                affectedEnemies.RemoveAt(i);
            }
        }
    }

    private void ApplyStunAndKnockback(Enemy enemy, bool isInitial)
    {
        // 确保敌人进入眩晕状态
        if (enemy.stateMachine.currentState != enemy.dizzyState)
        {
            enemy.stateMachine.ChangeState(enemy.dizzyState);
            enemy.ApplyStun(stunDuration);
        }

        // 应用更强烈的击退效果
        float calculatedForce = knockbackForce * knockbackMultiplier;

        // 使用AddForce替代SetVelocity以获得更真实的物理效果
        Vector2 knockbackDirection = new Vector2(direction, 0.2f); // 轻微向上击退
        enemy.rb.AddForce(knockbackDirection * calculatedForce, ForceMode2D.Impulse);

        // 添加受击特效
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                hitEffectPrefab,
                enemy.transform.position,
                Quaternion.identity
            );
            Destroy(effect, hitEffectDuration);
        }

        // 首次接触时应用额外击退
        if (isInitial)
        {
            enemy.rb.AddForce(knockbackDirection * calculatedForce * 1.5f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            ApplyStunAndKnockback(enemy, true);
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

        Destroy(gameObject);
    }
}