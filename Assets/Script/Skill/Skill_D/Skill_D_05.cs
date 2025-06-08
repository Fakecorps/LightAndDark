using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_05 : Skill
{
    public GameObject UltFieldPrefab;
    private float skillDuration;  // 添加技能持续时间变量
    private Coroutine endUltCoroutine;  // 用于存储协程引用

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
        GameObject ultFieldObj = Instantiate(UltFieldPrefab, player.transform.position, player.transform.rotation);

        UltField ultField = ultFieldObj.GetComponent<UltField>();
        if (ultField != null)
        {
            skillDuration = ultField.duration;  // 使用字段的持续时间

            // 通知玩家技能激活
            player.SetUltActiveState(true);

            // 启动技能结束协程
            if (endUltCoroutine != null) StopCoroutine(endUltCoroutine);
            endUltCoroutine = StartCoroutine(EndUltSkill());
        }
    }
    private IEnumerator EndUltSkill()
    {
        yield return new WaitForSeconds(skillDuration);
        player.SetUltActiveState(false);
    }
}
