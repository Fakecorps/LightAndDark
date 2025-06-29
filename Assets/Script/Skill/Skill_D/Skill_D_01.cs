using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_01 : Skill
{
    Vector2 boxSize = new Vector2(1f, 2f);
    Vector2 rayStart;
    Vector2 DashDirection;
    public float DashDistance;
    public float chaseDisableTime;
    public LayerMask enemyLayerMask;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        rayStart = player.attackCheckSpot.position;
        DashDirection = Vector2.right * player.getFacingDir();
        StartCoroutine(DarkDash());

    }

    protected override void Update()
    {
        base.Update();
    }

    private IEnumerator DarkDash()
    {
        DashMethod();

        // 立即更新检测起点
        Vector2 newRayStart = player.attackCheckSpot.position;

        // 使用OverlapBox检测当前位置敌人
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            newRayStart + DashDirection * DashDistance * 0.5f, // 中心点
            new Vector2(DashDistance, boxSize.y), // 尺寸
            0f,
            enemyLayerMask
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy_Goblin>(out var goblin))
            {
                goblin.enableChase = false;
                goblin.stateMachine.ChangeState(goblin.idleState);
                yield return new WaitForSeconds(chaseDisableTime);
                goblin.enableChase = true;
            }
            if (hit.TryGetComponent<Enemy_Archer>(out var archer))
            {
                archer.enableChase = false;
                archer.stateMachine.ChangeState(archer.idleState);
                yield return new WaitForSeconds(chaseDisableTime);
                archer.enableChase = true;
            }
            if (hit.TryGetComponent<Enemy_DarkBoss>(out var dboss))
            {
                dboss.enableChase = false;
                dboss.stateMachine.ChangeState(dboss.idleState);
                yield return new WaitForSeconds(chaseDisableTime);
                dboss.enableChase = true;
            }
        }

        yield break; // 无需等待

    }
    private void DashMethod()
    {
        RaycastHit2D wallCheck = Physics2D.Raycast(
    player.transform.position,
    DashDirection,
    DashDistance,
    LayerMask.GetMask("Ground") // 地面层级
);

        // 实际移动距离 = 预设距离与障碍物距离的较小值
        float actualDistance = wallCheck.collider != null ?
            wallCheck.distance : DashDistance;

        // 通过Rigidbody移动（保留碰撞）
        player.rb.MovePosition(
            player.rb.position + DashDirection * actualDistance
        );
    }
}
