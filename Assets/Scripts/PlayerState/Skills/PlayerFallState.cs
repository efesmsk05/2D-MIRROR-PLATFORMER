using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerNetworkManager player, PlayerMovement playerMovement) : base(player, playerMovement)
    {
    }
    public override void Enter()
    {
        playerMovement.SetFastFallGravity();
    }

    public override void FixedUpdate()
    {
        playerMovement.PlayerMove();
    }

    public override void Update()
    {

        if (player.inputManager.isDashPressed)
        {
            player.inputManager.DashReset();
            player.ChangeState(new PlayerDashState(player , playerMovement));
        }



        if (playerMovement.IsTouchingWall() && !playerMovement.IsGrounded())
        {
            player.ChangeState(new PlayerWallState(player , playerMovement));
        }


        if (playerMovement.IsGrounded())
        {
            playerMovement.SetNormalGravity();

            if (player.inputManager.isMoving)
            {
                player.ChangeState(new WalkState(player , playerMovement));
            }
            else
            {
                player.ChangeState(new IdleState(player , playerMovement));
            }
        }


    }

    public override void Exit()
    {
        player.animator.SetBool("isJumping", false);
        player.animator.SetBool("isDashing", false);


    }



}
