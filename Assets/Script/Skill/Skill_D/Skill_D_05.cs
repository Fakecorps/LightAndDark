using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_05 : Skill
{
    public GameObject UltFieldPrefab;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        CreateUltField();
    }

    protected override void Update()
    {
        base.Update();
    }

    void CreateUltField()
    { 
        GameObject UltField = Instantiate(UltFieldPrefab, player.transform.position, player.transform.rotation);
    }
}
