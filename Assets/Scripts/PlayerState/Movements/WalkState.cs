using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerNetworkManager player, PlayerMovement playerMovement) : base(player, playerMovement) { }


    public override void Enter()
    {

        player.animator.SetBool("isWalking" , true);

    }

    public override void Update()
    {
        if (player.inputManager.IsJumpBuffered() && playerMovement.IsGrounded())
        {
            player.inputManager.ConsumeJumpBuffer();
            player.ChangeState(new JumpState(player , playerMovement));

        }


        if (player.inputManager.isDashPressed)// dash atmýyor ve tuþa basýyor ise
        {
            player.inputManager.DashReset();
            player.ChangeState(new PlayerDashState(player , playerMovement));
        }

        if (!player.inputManager.isMoving && playerMovement.IsGrounded())
        {
            player.ChangeState(new IdleState(player , playerMovement));
        }

        if (player.inputManager.isMoving && !playerMovement.IsGrounded())
        {
            player.ChangeState(new PlayerFallState(player, playerMovement));

        }





    }

    public override void FixedUpdate()
    {
        playerMovement.PlayerMove();

    }

    public override void Exit()
    {
        player.animator.SetBool("isWalking" , false);
    }

}



