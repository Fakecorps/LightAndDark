using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_02 : Skill
{
    [Header("Chain Settings")]
    public Transform castPoint;          // 光链发射点
    public GameObject chainPrefab;       // 光链预制体
    public LayerMask enemyLayer;         // 敌人层级
    public float chainRange = 8f;        // 光链射程
    public float pullForce = 35f;        // 拉取力量
    public float stunDuration = 1f;      // 眩晕时间
    public float chainWidth = 0.3f;      // 光链宽度

    private GameObject activeChain;      // 当前激活的光链
    private Transform capturedEnemy;     // 被捕获的敌人
    public static Skill_L_02 skill_L_02 ;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Debug.Log("Skill_L_02");

        if (player == null)
            player = Player.ActivePlayer;

        // 触发动画
        player.anim.SetTrigger("Skill_L_02");
    }

    // 动画事件调用（在动画关键帧添加事件）
    public void OnCastAnimationEvent()
    {
        Debug.Log("Cast");

        if (player == null) return;

        // 创建光链实例
        activeChain = Instantiate(chainPrefab, castPoint.position, castPoint.rotation, castPoint);

        // 射线检测敌人
        RaycastHit hit;
        if (Physics.Raycast(castPoint.position, castPoint.forward, out hit, chainRange, enemyLayer))
        {
            capturedEnemy = hit.transform;
            Enemy enemy = capturedEnemy.GetComponent<Enemy>();

            if (enemy != null)
            {
                // 应用眩晕效果
                enemy.ApplyStun(stunDuration);

                // 开始拉取协程
                StartCoroutine(PullEnemy(capturedEnemy));
            }

            // 更新光链视觉
            UpdateChainVisual(hit.point);
        }
        else
        {
            // 没有命中敌人，光链延伸到最大距离
            Vector3 endPoint = castPoint.position + castPoint.forward * chainRange;
            UpdateChainVisual(endPoint);

            // 启动回收协程
            StartCoroutine(RetractChain());
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    private void UpdateChainVisual(Vector3 endPoint)
    {
        if (!activeChain) return;

        LineRenderer line = activeChain.GetComponent<LineRenderer>();
        if (line == null) return;

        line.SetPosition(0, castPoint.position);
        line.SetPosition(1, endPoint);

        // 动态宽度曲线
        line.widthCurve = new AnimationCurve(
            new Keyframe(0, chainWidth * 0.3f),
            new Keyframe(0.5f, chainWidth),
            new Keyframe(1, chainWidth * 0.3f)
        );
    }

    private IEnumerator PullEnemy(Transform enemy)
    {
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb == null) yield break;

        // 目标位置
        Vector3 targetPosition = player.transform.position + player.transform.forward * 10f;

        float pullDuration = 0.4f;
        float timer = 0;

        // 临时禁用碰撞避免卡住
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider) enemyCollider.enabled = false;

        while (timer < pullDuration)
        {
            if (enemy == null) yield break;

            timer += Time.deltaTime;
            Vector3 direction = (targetPosition - enemy.position).normalized;
            enemyRb.velocity = direction * pullForce;

            // 更新光链末端位置
            UpdateChainVisual(enemy.position);

            yield return null;
        }

        // 恢复碰撞器
        if (enemyCollider) enemyCollider.enabled = true;

        // 回收光链
        StartCoroutine(RetractChain());
    }

    private IEnumerator RetractChain()
    {
        if (!activeChain) yield break;

        LineRenderer line = activeChain.GetComponent<LineRenderer>();
        Vector3 startPos = line.GetPosition(0);
        Vector3 endPos = line.GetPosition(1);

        float duration = 0.2f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            line.SetPosition(1, Vector3.Lerp(endPos, startPos, t));
            yield return null;
        }

        Destroy(activeChain);
        activeChain = null;
        capturedEnemy = null;
    }

    protected override void Start()
    {
        base.Start();
        skill_L_02 = this;
    }
}
