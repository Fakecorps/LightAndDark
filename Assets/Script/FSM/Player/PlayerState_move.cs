using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_move : PlayerState_Grounded
{
    public PlayerState_move(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        player.SetVelocity(player.AxisInput.x * player.moveSpeed, player.rb.velocity.y); 

        if (Mathf.Abs(player.AxisInput.x) < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (player.inputControl.Player.Jump.triggered)
        {
            Debug.Log("OK");
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.inputControl.Player.Attack.triggered)
        {
            stateMachine.ChangeState(player.primaryAttack);
        }
    }
}
