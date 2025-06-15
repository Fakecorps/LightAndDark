using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_04 : Skill
{
    public static Skill_L_04 Instance;

    [Header("Wave Settings")]
    public GameObject wavePrefab;
    public Transform spawnPoint;
    public float waveSpeed = 3f;
    public float maxDistance = 15f;

    [Header("Wave Size")]
    public float waveScaleX = 5f;  // 水平缩放
    public float waveScaleY = 7f;  // 垂直缩放

    [Header("Timing Settings")]
    public float animationDuration = 1.2f;
    public float waveSpawnDelay = 0.8f;

    [Header("Visual Effects")]
    public GameObject slashEffect;

    private bool isCasting;

    protected override void Start()
    {
        Instance = this;
        // 设置默认缩放值
        waveScaleX = 5f;
        waveScaleY = 7f;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (player == null)
            player = Player.ActivePlayer;

        player.anim.SetTrigger("Skill_L_04");
        player.SetCastingSkill(true);
        isCasting = true;

        StartCoroutine(SkillSequence());
    }

    private IEnumerator SkillSequence()
    {
        yield return new WaitForSeconds(animationDuration);
        player.SetCastingSkill(false);
        yield return new WaitForSeconds(waveSpawnDelay);
        SpawnWave();
        isCasting = false;
    }

    private void SpawnWave()
    {
        if (wavePrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("剑气波预制体或生成点未设置");
            return;
        }

        int facingDir = player.getFacingDir();
        Vector3 spawnPosition = spawnPoint.position;

        // 生成横斩特效
        if (slashEffect != null)
        {
            GameObject effect = Instantiate(
                slashEffect,
                spawnPosition,
                Quaternion.identity
            );

            // 设置特效方向
            Vector3 effectScale = effect.transform.localScale;
            effectScale.x = Mathf.Abs(effectScale.x) * (facingDir == 1 ? 1 : -1);
            effect.transform.localScale = effectScale;

            Destroy(effect, 1f);
        }

        // 生成剑气波
        GameObject wave = Instantiate(
            wavePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // 设置剑气波大小
        wave.transform.localScale = new Vector3(
            waveScaleX * (facingDir == 1 ? 1 : -1),
            waveScaleY,
            1
        );

        // 设置移动属性
        Projectile_SlashWave waveScript = wave.GetComponent<Projectile_SlashWave>();
        if (waveScript != null)
        {
            waveScript.moveSpeed = waveSpeed;
            waveScript.maxDistance = maxDistance;
            waveScript.SetDirection(facingDir);

            // 设置碰撞体大小（使用缩放因子）
            waveScript.SetColliderSize(waveScaleX, waveScaleY);

            // 可选：调整碰撞体缩放因子
            //waveScript.colliderScaleFactor = 0.7f; // 0.7意味着碰撞体是视觉大小的70%
        }
    }
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();
            player.SetCastingSkill(false);
            isCasting = false;
        }
    }
}