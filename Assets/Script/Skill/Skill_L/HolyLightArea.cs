using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyLightArea : MonoBehaviour
{
    [Header("Area Settings")]
    public float radius = 5f;            // ����뾶
    public float duration = 3f;          // ���ܳ���ʱ��
    public float damageInterval = 0.5f;  // �˺����
    public int damagePerTick = 20;       // ÿ���˺���
    public bool stunEnemies = true;      // �Ƿ�ѣ�ε���

    private float timer;
    private float nextDamageTime;
    private List<Enemy> affectedEnemies = new List<Enemy>();

    private void Start()
    {
        timer = duration;
        nextDamageTime = Time.time + damageInterval;

        // ��ʼ��������ڵĵ���
        DetectInitialEnemies();

        // ��ʼЭ�̴�������Ч��
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
        // ����ʱ����Ч��
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // �����˺�
            if (Time.time >= nextDamageTime)
            {
                ApplyDamage();
                nextDamageTime = Time.time + damageInterval;
            }

            // ����µ���
            DetectNewEnemies();

            yield return null;
        }

        // ��������Ч��
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
        // �������е��˵�ѣ��״̬
        foreach (Enemy enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.EndStun();
            }
        }

        // ��������
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
            // �뿪������������ѣ��
            enemy.EndStun();
            affectedEnemies.Remove(enemy);
        }
    }

    // ���ӻ�����
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}