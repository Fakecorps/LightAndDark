using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_05 : Skill
{
    public static Skill_L_05 Instance;

    [Header("Area Settings")]
    public GameObject holyAreaPrefab;    // 圣光区域预制体
    public float areaRadius = 5f;        // 区域半径
    public float castTime = 2f;          // 施法时间（引导时间）

    private GameObject castEffect;       // 施法特效
    private bool isCasting;              // 是否正在施法

    protected override void Start()
    {
        Instance = this;
    }

    public override void UseSkill()
    {
        //if (!CanUseSkill())
        //{
        //    return;
        //}

        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        // 触发动画
        player.anim.SetTrigger("Skill_L_05");

        // 设置施法状态
        player.SetCastingSkill(true);
        isCasting = true;

        // 开始施法协程
        StartCoroutine(CastRoutine());

        Debug.Log("光域决技能激活");
    }

    private IEnumerator CastRoutine()
    {
        // 创建施法特效（双手举剑）
        //if (castEffect != null)
        //{
        //    castEffect = Instantiate(/* 施法特效预制体 */, player.transform.position, Quaternion.identity);
        //}

        // 施法引导时间
        yield return new WaitForSeconds(castTime);

        // 施法结束，创建圣光区域
        CreateHolyArea();

        // 结束施法状态
        player.SetCastingSkill(false);
        isCasting = false;

        // 销毁施法特效
        if (castEffect != null)
        {
            Destroy(castEffect);
        }
    }

    private void CreateHolyArea()
    {
        if (holyAreaPrefab == null)
        {
            Debug.LogWarning("圣光区域预制体未设置");
            return;
        }

        // 在玩家位置创建圣光区域
        GameObject holyArea = Instantiate(
            holyAreaPrefab,
            player.transform.position,
            Quaternion.identity
        );

        // 设置区域属性
        HolyLightArea areaScript = holyArea.GetComponent<HolyLightArea>();
        if (areaScript != null)
        {
            areaScript.radius = areaRadius;
        }

        Debug.Log($"圣光区域创建于 {player.transform.position}");
    }

    // 技能中断处理
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            // 销毁施法特效
            if (castEffect != null)
            {
                Destroy(castEffect);
            }

            // 结束施法状态
            player.SetCastingSkill(false);
            isCasting = false;

            Debug.Log("技能中断");
        }
    }
}