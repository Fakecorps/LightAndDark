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

    // ���������ܶ����¼�
    public void CastChainTrigger()
    {
        // ��ȡ������������������¼�
        Skill_L_02 chainSkill = Skill_L_02.skill_L_02;
        if (chainSkill != null)
        {
            chainSkill.OnCastAnimationEvent();
        }
        else
        {
            Debug.LogWarning("δ�ҵ�Skill_L_02���");
        }
    }


}
