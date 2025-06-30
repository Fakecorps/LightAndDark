using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();
    private Skill_L_03 skill;

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
                hit.GetComponent<Enemy>().TakeDamage(player.Normal_Attack_Damage);
            }
        }
    }

    // 新增：技能动画事件
    public void CastChainTrigger()
    {
        // 获取技能组件并触发动画事件
        Skill_L_02 chainSkill = Skill_L_02.skill_L_02;
        if (chainSkill != null)
        {
            chainSkill.OnCastAnimationEvent();
        }
        else
        {
            Debug.LogWarning("未找到Skill_L_02组件");
        }
    }


}
