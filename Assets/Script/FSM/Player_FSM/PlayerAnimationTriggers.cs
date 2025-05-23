using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AniamtionTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheckSpot.position, player.attackRadius);
        foreach (var hit in colliders)
        { 
            if(hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().TakeDamage();
            }
        }
    }
}
