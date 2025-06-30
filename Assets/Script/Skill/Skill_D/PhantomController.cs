using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomController : MonoBehaviour
{
    private Enemy target;
    public int damage = 5;
    public float duration;
    public float baseScale = 0.01f;

    public void Initialize(Enemy target, int damage, float duration)
    {
        this.target = target;
        this.damage = damage;
        this.duration = duration;

        Vector3 enemyPos = target.transform.position;
        transform.position = new Vector3(
            transform.position.x + 1,
            enemyPos.y,  // 关键修改：使用敌人的Y轴高度
            enemyPos.z
        );

        // 设置缩放（增大基础值并添加方向控制）
        float direction = Mathf.Sign(enemyPos.x - transform.position.x );
        transform.localScale = new Vector3(
            direction * baseScale,  // 方向×基础缩放
            baseScale,               // 统一Y轴缩放
            baseScale                // 统一Z轴缩放
        );

        Invoke("AttackTarget", duration * 0.5f); // 半程时攻击
        Destroy(gameObject, duration);
    }

    private void AttackTarget()
    {
        if (target != null && target.gameObject != null)
        {
            target.TakeDamage(damage);
            transform.localScale *= 1.2f;
        }
        else
        {
            // 目标已销毁，销毁幻影
            Destroy(gameObject);
        }
    }
}
