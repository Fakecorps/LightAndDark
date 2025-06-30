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

    [Header("Timing Settings")]
    public float animationDuration = 0.5f;
    public float waveSpawnDelay = 0.5f;

    [Header("Visual Effects")]
    public GameObject slashEffect;

    private bool isCasting;

    protected override void Start()
    {
        Instance = this;
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
            Debug.LogWarning("������Ԥ��������ɵ�δ����");
            return;
        }

        int facingDir = player.getFacingDir();
        Vector3 spawnPosition = spawnPoint.position;

        // ���ɺ�ն��Ч
        if (slashEffect != null)
        {
            GameObject effect = Instantiate(
                slashEffect,
                spawnPosition,
                Quaternion.identity
            );

            // ������Ч����
            Vector3 effectScale = effect.transform.localScale;
            effectScale.x = Mathf.Abs(effectScale.x) * (facingDir == 1 ? 1 : -1);
            effect.transform.localScale = effectScale;

            Destroy(effect, 1f);
        }

        // ���ɽ�������ʹ��Ԥ����ԭʼ��С��
        GameObject wave = Instantiate(
            wavePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // ���÷��򣨽���תX�ᣩ
        Vector3 waveScale = wave.transform.localScale;
        waveScale.x = Mathf.Abs(waveScale.x) * (facingDir == 1 ? 1 : -1);
        wave.transform.localScale = waveScale;

        // �����ƶ�����
        Projectile_SlashWave waveScript = wave.GetComponent<Projectile_SlashWave>();
        if (waveScript != null)
        {
            waveScript.moveSpeed = waveSpeed;
            waveScript.maxDistance = maxDistance;
            waveScript.SetDirection(facingDir);
        }
        else
        {
            Debug.LogWarning("������Ԥ����ȱ��Projectile_SlashWave���");
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