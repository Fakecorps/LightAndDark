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
    public float knockbackForce = 1f; // ���ӻ�������
    public LayerMask enemyLayer;
    public float stunDuration = 0.5f;

    [Header("Visual Effects")]
    public GameObject hitEffectPrefab; // �ܻ���Ч
    public float hitEffectDuration = 0.2f;

    private Vector3 startPosition;
    private float nextDamageTime;
    private List<Enemy> affectedEnemies = new List<Enemy>();
    private int direction = 1;
    private bool isDestroying = false;
    private float knockbackMultiplier = 1.0f; // ���˱���

    public void SetDirection(int dir)
    {
        direction = dir;

        // ���ݷ���������鷽��
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Vector3 scale = sr.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir == 1 ? 1 : -1);
            sr.transform.localScale = scale;
        }
    }

    // ���û��˱��ʣ���ѡ��
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

        // �ƶ�
        Vector3 moveDirection = Vector3.right * direction;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // ����������
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            DestroyWave();
            return;
        }

        // �����˺��ͻ���
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

        // �����ⷶΧ��ȷ���������
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
                // ����˺�
                affectedEnemies[i].TakeDamage(damagePerTick);

                // Ӧ�û��˺�ѣ��
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
        // ȷ�����˽���ѣ��״̬
        if (enemy.stateMachine.currentState != enemy.dizzyState)
        {
            enemy.stateMachine.ChangeState(enemy.dizzyState);
            enemy.ApplyStun(stunDuration);
        }

        // Ӧ�ø�ǿ�ҵĻ���Ч��
        float calculatedForce = knockbackForce * knockbackMultiplier;

        // ʹ��AddForce���SetVelocity�Ի�ø���ʵ������Ч��
        Vector2 knockbackDirection = new Vector2(direction, 0.2f); // ��΢���ϻ���
        enemy.rb.AddForce(knockbackDirection * calculatedForce, ForceMode2D.Impulse);

        // ����ܻ���Ч
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                hitEffectPrefab,
                enemy.transform.position,
                Quaternion.identity
            );
            Destroy(effect, hitEffectDuration);
        }

        // �״νӴ�ʱӦ�ö������
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