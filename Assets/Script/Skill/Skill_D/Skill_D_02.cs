using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_D_02 : Skill
{
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private float daggerSpeed = 20f;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        Player player = Player.ActivePlayer;
        if (player == null) return;

        Vector3 spawnPos = player.attackCheckSpot.position;
        GameObject dagger = Instantiate(daggerPrefab, spawnPos, Quaternion.identity);
        dagger.GetComponent<Dagger>().SetupDagger(player.getFacingDir(), daggerSpeed);
    }

    protected override void Update()
    {
        base.Update();
    }
}
