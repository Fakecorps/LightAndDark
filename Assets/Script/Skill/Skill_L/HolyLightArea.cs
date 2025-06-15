using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyLightArea : MonoBehaviour
{
    [Header("Area Settings")]
    public float radius = 5f;            // 区域半径
    public float duration = 3f;          // 技能持续时间
    public float damageInterval = 0.5f;  // 伤害间隔
    public int damagePerTick = 20;       // 每次伤害量
    public bool stunEnemies = true;      // 是否眩晕敌人

    private float timer;
    private float nextDamageTime;
    private List<Enemy> affectedEnemies = new List<Enemy>();

    private void Start()
    {
        timer = duration;
        nextDamageTime = Time.time + damageInterval;

        // 初始检测区域内的敌人
        DetectInitialEnemies();

        // 开始协程处理区域效果
        StartCoroutine(AreaRoutine());
    }

    private void DetectInitialEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                ApplyStun(enemy);
            }
        }
    }

    private IEnumerator AreaRoutine()
    {
        // 持续时间内效果
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // 处理伤害
            if (Time.time >= nextDamageTime)
            {
                ApplyDamage();
                nextDamageTime = Time.time + damageInterval;
            }

            // 检测新敌人
            DetectNewEnemies();

            yield return null;
        }

        // 结束区域效果
        EndAreaEffect();
    }

    private void DetectNewEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                ApplyStun(enemy);
            }
        }
    }

    private void ApplyStun(Enemy enemy)
    {
        if (stunEnemies)
        {
            enemy.ApplyStun(duration);
        }
    }

    private void ApplyDamage()
    {
        for (int i = affectedEnemies.Count - 1; i >= 0; i--)
        {
            if (affectedEnemies[i] != null)
            {
                affectedEnemies[i].TakeDamage(damagePerTick);
            }
            else
            {
                affectedEnemies.RemoveAt(i);
            }
        }
    }

    private void EndAreaEffect()
    {
        // 结束所有敌人的眩晕状态
        foreach (Enemy enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.EndStun();
            }
        }

        // 销毁区域
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            ApplyStun(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            // 离开区域立即结束眩晕
            enemy.EndStun();
            affectedEnemies.Remove(enemy);
        }
    }

    // 可视化区域
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}