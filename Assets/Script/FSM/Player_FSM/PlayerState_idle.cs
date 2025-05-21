using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_idle : PlayerState_Grounded
{
    public PlayerState_idle(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();  

        if (Mathf.Abs(player.AxisInput.x) > 0.1f && !player.isBusy)
        {
            stateMachine.ChangeState(player.moveState);
        }

        if (player.inputControl.Player.Jump.triggered&&player.isGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.inputControl.Player.Attack.triggered)
        {
            stateMachine.ChangeState(player.primaryAttack);
        }
    }
}
