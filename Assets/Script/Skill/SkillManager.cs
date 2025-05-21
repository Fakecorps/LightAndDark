using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
#region Skill
    public Skill_L_01 skill_L_01;
    public Skill_L_02 skill_L_02;
    public Skill_L_03 skill_L_03;
    public Skill_L_04 skill_L_04;
    public Skill_L_05 skill_L_05;
    public Skill_D_01 skill_D_01;
    public Skill_D_02 skill_D_02;
    public Skill_D_03 skill_D_03;
    public Skill_D_04 skill_D_04;
    public Skill_D_05 skill_D_05;
#endregion


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        
    }

    public void UseSkill(int skillID)
    {
        if (PlayerManager.instance.isPlayerLight)
        {
            switch (skillID)
            {
                case 0:
                    skill_L_01.CanUseSkill();
                    break;
                case 1:
                    skill_L_02.CanUseSkill();
                    break;
                case 2:
                    skill_L_03.CanUseSkill();
                    break;
                case 3:
                    skill_L_04.CanUseSkill();
                    break;
                case 4:
                    skill_L_05.CanUseSkill();
                    break;
            }
        }
        else
        {
            switch (skillID)
            {
                case 0:
                    skill_D_01.CanUseSkill();
                    break;
                case 1:
                    skill_D_02.CanUseSkill();
                    break;
                case 2:
                    skill_D_03.CanUseSkill();
                    break;
                case 3:
                    skill_D_04.CanUseSkill();
                    break;
                case 4:
                    skill_D_05.CanUseSkill();
                    break;


            }
        }

    }

}
