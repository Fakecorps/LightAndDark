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

    #region Unlock Skill
    public bool UnlockSkill_L_01 = true;
    public bool UnlockSkill_L_02 = true;
    public bool UnlockSkill_L_03 = true;
    public bool UnlockSkill_L_04 = true;
    public bool UnlockSkill_L_05 = true;
    public bool UnlockSkill_D_01 = true;
    public bool UnlockSkill_D_02 = true;
    public bool UnlockSkill_D_03 = true;
    public bool UnlockSkill_D_04 = true;
    public bool UnlockSkill_D_05 = true;
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
        if (PlayerManager.Instance.isPlayerLight)
        {
            switch (skillID)
            {
                case 0:
                    if (UnlockSkill_L_01)
                    {
                        skill_L_01.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 1:
                    if (UnlockSkill_L_02)
                    { 
                        skill_L_02.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 2:
                    if (UnlockSkill_L_03)
                    {
                        skill_L_03.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 3:
                    if (UnlockSkill_L_04)
                    {
                        skill_L_04.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 4:
                    if (UnlockSkill_L_05)
                    {
                        skill_L_05.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
            }
        }
        else
        {
            switch (skillID)
            {
                case 0:
                    if (UnlockSkill_D_01)
                    {
                        skill_D_01.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 1:
                    if (UnlockSkill_D_02)
                    {
                        skill_D_02.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 2:
                    if (UnlockSkill_D_03)
                    {
                        skill_D_03.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 3:
                    if (UnlockSkill_D_04)
                    {
                        skill_D_04.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }
                    break;
                case 4:
                    if (UnlockSkill_D_05)
                    {
                        skill_D_05.CanUseSkill();
                    }
                    else
                    {
                        Debug.Log("Skill not unlocked");
                    }

                    break;


            }
        }

    }

}
