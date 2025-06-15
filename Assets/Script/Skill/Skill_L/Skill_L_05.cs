using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_L_05 : Skill
{
    public static Skill_L_05 Instance;

    [Header("Area Settings")]
    public GameObject holyAreaPrefab;    // ʥ������Ԥ����
    public float areaRadius = 5f;        // ����뾶
    public float castTime = 2f;          // ʩ��ʱ�䣨����ʱ�䣩

    private GameObject castEffect;       // ʩ����Ч
    private bool isCasting;              // �Ƿ�����ʩ��

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

        // ��������
        player.anim.SetTrigger("Skill_L_05");

        // ����ʩ��״̬
        player.SetCastingSkill(true);
        isCasting = true;

        // ��ʼʩ��Э��
        StartCoroutine(CastRoutine());

        Debug.Log("��������ܼ���");
    }

    private IEnumerator CastRoutine()
    {
        // ����ʩ����Ч��˫�־ٽ���
        //if (castEffect != null)
        //{
        //    castEffect = Instantiate(/* ʩ����ЧԤ���� */, player.transform.position, Quaternion.identity);
        //}

        // ʩ������ʱ��
        yield return new WaitForSeconds(castTime);

        // ʩ������������ʥ������
        CreateHolyArea();

        // ����ʩ��״̬
        player.SetCastingSkill(false);
        isCasting = false;

        // ����ʩ����Ч
        if (castEffect != null)
        {
            Destroy(castEffect);
        }
    }

    private void CreateHolyArea()
    {
        if (holyAreaPrefab == null)
        {
            Debug.LogWarning("ʥ������Ԥ����δ����");
            return;
        }

        // �����λ�ô���ʥ������
        GameObject holyArea = Instantiate(
            holyAreaPrefab,
            player.transform.position,
            Quaternion.identity
        );

        // ������������
        HolyLightArea areaScript = holyArea.GetComponent<HolyLightArea>();
        if (areaScript != null)
        {
            areaScript.radius = areaRadius;
        }

        Debug.Log($"ʥ�����򴴽��� {player.transform.position}");
    }

    // �����жϴ���
    public void CancelSkill()
    {
        if (isCasting)
        {
            StopAllCoroutines();

            // ����ʩ����Ч
            if (castEffect != null)
            {
                Destroy(castEffect);
            }

            // ����ʩ��״̬
            player.SetCastingSkill(false);
            isCasting = false;

            Debug.Log("�����ж�");
        }
    }
}