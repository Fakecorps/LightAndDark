using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_03 : Skill
{
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Skill_D_03");
    }

    protected override void Update()
    {
        base.Update();
    }
}
