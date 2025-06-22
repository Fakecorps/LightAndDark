using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public float explosionRadius;
    public int explosionDamage;

    // 添加自动销毁设置
    [SerializeField] private float autoDestroyTime = 10f; // 默认10秒后自动销毁
    private Coroutine autoDestroyCoroutine;

    // 添加销毁事件
    public event System.Action OnDestroyed;

    public void Initialize(float radius, int damage, float lifetime = 10f)
    {
        explosionRadius = radius;
        explosionDamage = damage;
        autoDestroyTime = lifetime;

        // 启动自动销毁计时器
        autoDestroyCoroutine = StartCoroutine(AutoDestroyRoutine());
    }

    private IEnumerator AutoDestroyRoutine()
    {
        yield return new WaitForSeconds(autoDestroyTime);

        // 时间到，自动销毁
        ExplodeWithoutDamage();
    }

    public void Explosion()
    {
        // 执行爆炸伤害逻辑
        ExplodeWithDamage();
    }

    private void ExplodeWithDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().TakeDamage(explosionDamage);
                hit.GetComponent<Enemy>().stateMachine.ChangeState(hit.GetComponent<Enemy>().dizzyState);
            }
        }

        // 停止自动销毁计时
        if (autoDestroyCoroutine != null)
        {
            StopCoroutine(autoDestroyCoroutine);
            autoDestroyCoroutine = null;
        }

        DestroySelf();
    }

    private void ExplodeWithoutDamage()
    {
        // 只是消失，不造成伤害
        DestroySelf();
    }

    private void DestroySelf()
    {
        // 触发销毁事件
        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }

    // 添加 OnDestroy 确保任何销毁方式都触发事件
    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    // 可选：添加可视化倒计时（调试用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}