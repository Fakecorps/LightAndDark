using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Animation_Trigger : MonoBehaviour
{
    private Enemy_Goblin enemy => GetComponentInParent<Enemy_Goblin>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[]colliders = Physics2D.OverlapCircleAll(enemy.attackCheckSpot.position, enemy.attackRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Player>().TakeDamage(enemy.damage);
            }
        }
    }

    protected void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    protected void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
