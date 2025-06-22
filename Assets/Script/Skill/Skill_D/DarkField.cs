using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 添加Linq命名空间

public class DarkField : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private float damageInterval = 0.2f;
    [SerializeField] private int damagePerTick = 5;

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
        float duration = 0.2f;
        float timer = 0;

        while (timer < duration)
        {
            float progress = timer / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * radius * 2, progress);
            fieldVisual.color = new Color(
                fieldColor.r,
                fieldColor.g,
                fieldColor.b,
                Mathf.Lerp(0, fieldColor.a, progress)
            );

            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOutField());
    }

    private IEnumerator DamageRoutine()
    {
        while (true)
        {
            // 创建集合的副本进行遍历
            List<Enemy> enemiesToDamage = new List<Enemy>();

            // 清理无效引用并创建副本
            affectedEnemies.RemoveWhere(enemy => enemy == null);
            enemiesToDamage.AddRange(affectedEnemies);

            foreach (Enemy enemy in enemiesToDamage)
            {
                // 跳过已销毁或无效的敌人
                if (enemy == null || !enemy.gameObject.activeSelf) continue;

                enemy.enableChase = false;

                if (enemy is Enemy_Goblin goblin)
                {
                    // 确保哥布林没有被眩晕或死亡
                    if (!goblin.isDead)
                    {
                        goblin.stateMachine.ChangeState(goblin.knockbackState);
                    }
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
            // 只添加存活的敌人
            if (!enemy.isDead)
            {
                affectedEnemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // 安全移除敌人
            affectedEnemies.Remove(enemy);

            // 只恢复存活的敌人
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                enemy.enableChase = true;
            }
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

        // 技能结束时销毁自身
        Destroy(gameObject);
    }
}