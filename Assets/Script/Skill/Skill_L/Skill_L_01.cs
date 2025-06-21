using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_01 : Skill
{
    public static Skill_L_01 Instance;
    public LayerMask enemyLayerMask;
    public Vector2 boxSize;
    public int ParryDamage;
    public Vector2 KnockBackForce;
    private void Awake()
    {
        Instance = this;
    }
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Skill_L_01");
        player.rb.velocity = Vector2.zero;
        if (!player.parrySuccess)
            return;
        StartCoroutine(ParrySkill());
        player.parrySuccess = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    private IEnumerator ParrySkill()
    {
        Vector2 newRayStart = player.attackCheckSpot.position;
        // 使用OverlapBox检测当前位置敌人
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            newRayStart, // 中心点
            new Vector2(boxSize.x, boxSize.y), // 尺寸
            0f,
            enemyLayerMask
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy_Goblin>(out var goblin))
            {
                goblin.stateMachine.ChangeState(goblin.dizzyState);
                goblin.TakeDamage(ParryDamage);
                goblin.SetVelocity(player.getFacingDir()*KnockBackForce.x, KnockBackForce.y);
            }
        }
        player.StateMachine.ChangeState(player.idleState);
        yield break; // 无需等待
    }
}
