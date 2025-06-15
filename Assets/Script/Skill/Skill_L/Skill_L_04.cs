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
    public float waveScaleX = 5f;  // ˮƽ����
    public float waveScaleY = 7f;  // ��ֱ����

    [Header("Timing Settings")]
    public float animationDuration = 1.2f;
    public float waveSpawnDelay = 0.8f;

    [Header("Visual Effects")]
    public GameObject slashEffect;

    private bool isCasting;

    protected override void Start()
    {
        Instance = this;
        // ����Ĭ������ֵ
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

        // ���ɽ�����
        GameObject wave = Instantiate(
            wavePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // ���ý�������С
        wave.transform.localScale = new Vector3(
            waveScaleX * (facingDir == 1 ? 1 : -1),
            waveScaleY,
            1
        );

        // �����ƶ�����
        Projectile_SlashWave waveScript = wave.GetComponent<Projectile_SlashWave>();
        if (waveScript != null)
        {
            waveScript.moveSpeed = waveSpeed;
            waveScript.maxDistance = maxDistance;
            waveScript.SetDirection(facingDir);

            // ������ײ���С��ʹ���������ӣ�
            waveScript.SetColliderSize(waveScaleX, waveScaleY);

            // ��ѡ��������ײ����������
            //waveScript.colliderScaleFactor = 0.7f; // 0.7��ζ����ײ�����Ӿ���С��70%
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