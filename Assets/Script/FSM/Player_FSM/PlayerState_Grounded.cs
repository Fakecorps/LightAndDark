using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Grounded : PlayerState
{
    public PlayerState_Grounded(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        if (player.inputControl.Player.Skill04.triggered)
        {
            Debug.Log("Parryed");
            player.rb.velocity = Vector2.zero;
            stateMachine.ChangeState(player.parryState);
        }
    }
}
