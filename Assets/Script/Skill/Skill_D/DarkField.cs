using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkField : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private float damageInterval = 0.1f;
    [SerializeField] private int damagePerTick = 5;
    [SerializeField] private float slowPercentage = 0.5f;

    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>();

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer fieldVisual;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fieldColor = new Color(0, 0, 0, 1f);

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = radius;
        fieldVisual.color = new Color(fieldColor.r, fieldColor.g, fieldColor.b, 0);
        StartCoroutine(ExpandField());
        StartCoroutine(DamageRoutine());
    }

    private IEnumerator ExpandField()
    {
        float duration = 1f;
        float timer = 0;

        // 同时控制缩放和透明度
        while (timer < duration)
        {
            float progress = timer / duration;

            // 缩放控制
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * radius * 2, progress);

            // 透明度控制
            fieldVisual.color = new Color(
                fieldColor.r,
                fieldColor.g,
                fieldColor.b,
                Mathf.Lerp(0, fieldColor.a, progress)
            );

            timer += Time.deltaTime;
            yield return null;
        }

        // 最终淡出效果
        StartCoroutine(FadeOutField());
    }

    private IEnumerator DamageRoutine()
    {
        while (true)
        {
            foreach (Enemy enemy in affectedEnemies)
            {
                if (enemy is Enemy_Goblin)
                { 
                    var goblin = enemy as Enemy_Goblin;
                    goblin.stateMachine.ChangeState(goblin.stunState);
                }
                enemy.TakeDamage(damagePerTick);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            affectedEnemies.Add(enemy);
            enemy.moveSpeed *= slowPercentage;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            affectedEnemies.Remove(enemy);
            enemy.moveSpeed /= slowPercentage;
        }
    }

    private IEnumerator FadeOutField()
    {
        float timer = 0;
        Color startColor = fieldVisual.color;

        while (timer < fadeDuration)
        {
            fieldVisual.color = Color.Lerp(startColor, Color.clear, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
