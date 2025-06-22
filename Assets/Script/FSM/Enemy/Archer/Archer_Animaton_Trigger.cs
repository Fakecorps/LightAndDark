using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Animaton_Trigger : MonoBehaviour
{
    private Enemy_Archer enemy => GetComponentInParent<Enemy_Archer>();

    //private void AnimationTrigger()
    //{
    //    enemy.AnimationTrigger();
    //}

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheckSpot.position, enemy.attackRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                var player = hit.GetComponent<Player>();

                if (player.isParrying && enemy.getFacingDir() != player.getFacingDir())
                {
                    Debug.Log("is Parryed");
                    player.ActivateParrySkill();
                }
                else
                {
                    player.TakeDamage(enemy.damage);
                }
            }
            if (hit.GetComponent<Decoy>() != null)
            {
                hit.GetComponent<Decoy>().Explosion();
            }
        }
    }

    protected void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    protected void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
