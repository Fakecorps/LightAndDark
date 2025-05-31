using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_04 : Skill
{
    public static Skill_D_04 Instance;
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float shadowFadeSpeed = 2.0f;
    private SpriteRenderer playerSprite;

    Vector2 boxSize = new Vector2(1f, 2f);
    Vector2 rayStart;
    Vector2 DashDirection;
    public float DashDistance;
    public LayerMask enemyLayerMask;

    public int Damage;
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
        playerSprite = PlayerManager.Instance.player.sr;
        if (!player.parrySuccess)
        return;
        DashDirection = Vector2.right * player.getFacingDir();
        StartCoroutine(ParrySkill());
        player.parrySuccess = false;
    }

    protected override void Update()
    {
        base.Update();
        
    }

    private IEnumerator ParrySkill()
    {
        DashMethod();
        CreateShadow();

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
                goblin.stateMachine.ChangeState(goblin.stunState);
                goblin.TakeDamage(Damage);
            }
        }

        yield break; // 无需等待
    }
    private void CreateShadow()
    {
        GameObject shadow = Instantiate(
           shadowPrefab,
           player.transform.position,
           player.transform.rotation
       );

        SpriteRenderer shadowSprite = shadow.GetComponent<SpriteRenderer>();
        shadowSprite.sprite = playerSprite.sprite;
        shadowSprite.flipX = playerSprite.flipX;

        StartCoroutine(FadeOutShadow(shadow, shadowSprite));
    }
    private IEnumerator FadeOutShadow(GameObject shadow, SpriteRenderer shadowSprite)
    {
        Color currentColor = shadowSprite.color;

        while (currentColor.a > 0.01f)
        {
            // 计算新的alpha值（基于当前颜色）
            float newAlpha = currentColor.a - shadowFadeSpeed * Time.deltaTime;

            // 更新当前颜色
            currentColor.a = newAlpha;
            shadowSprite.color = currentColor;

            yield return null; // 等待下一帧
        }

        // 透明度接近0时销毁对象
        Destroy(shadow);
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
