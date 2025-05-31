using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Parry : PlayerState
{
    private float parryTimer;
    private float parryDuration;
    public PlayerState_Parry(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.ZeroVelocity();
        player.isParrying = true;
        parryDuration = player.parryDuration;
        parryTimer = parryDuration;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        parryTimer -= Time.deltaTime;
        if (parryTimer <= 0)
        { 
            player.isParrying = false;
            stateMachine.ChangeState(player.idleState);
        }
    }
}
