using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState_PrimaryAttack : PlayerState
{
    private int comboCounter;
    private float lastTimeAttacked;
    private float comboWindow = 1;
    private float attackSpeed = 0.7f;
    public PlayerState_PrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2||Time.time>=lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }
        player.ZeroVelocity();
        Debug.Log(comboCounter);
        player.anim.SetInteger("ComboCounter", comboCounter);
    }

    public override void Exit()
    {
        base.Exit();
        player.ZeroVelocity();

        player.StartCoroutine("BusyFor", .1f);

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(player.getFacingDir() * attackSpeed, player.rb.velocity.y);
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
