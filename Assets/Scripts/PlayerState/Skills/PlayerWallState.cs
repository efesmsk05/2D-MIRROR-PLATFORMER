using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallState : PlayerState
{

    private float wallDirectionX;

    public PlayerWallState(PlayerNetworkManager player , PlayerMovement playerMovement) : base(player , playerMovement)
    {
    }

    public override void Enter()
    {
        player.rb.gravityScale = 0f;


        playerMovement.rb.velocity = new Vector2(0f, Mathf.Max(player.rb.velocity.y, -playerMovement.wallSlideSpeed));

        wallDirectionX = playerMovement.playerFacingDirection.x;

        base.Enter();
    }

    
    public override void Update()
    {
        if( player.inputManager.IsJumpBuffered() && !playerMovement.IsGrounded() && playerMovement.IsTouchingWall())
        {
            player.inputManager.ConsumeJumpBuffer();

            Vector2 jumpVelocity = new Vector2(

                -wallDirectionX * playerMovement.wallJumpHorizontalForce ,
                playerMovement.jumpForce * .8f
                    
                );

            player.ChangeState(new PlayerWallJumpState(player , jumpVelocity , playerMovement));

            
            return;
        }

        //if (player.inputManager.isDashPressed) 
        //{
        //    // Belki duvardan dash atmanýn bir bedeli vardýr (stamina vs.)
        //    player.ChangeState(new PlayerDashState(player));
        //    return;
        //}

    }


    public override void FixedUpdate()
    {
        float moveDir = player.inputManager.moveDir.x;

        if (moveDir != 0 && Mathf.Sign(moveDir) == -wallDirectionX)
        {
            player.ChangeState(new PlayerFallState(player , playerMovement));
            return;
        }


        if (playerMovement.IsTouchingWall() && !playerMovement.IsGrounded())
        {
            WallSlide();
        }
        else
        {
            player.ChangeState(new PlayerFallState(player , playerMovement));
        }
    }


    private void WallSlide()
    {

        float moveDir = player.inputManager.moveDir.x;

        bool isHoldingOn = (moveDir != 0f && Mathf.Sign(moveDir) == wallDirectionX);

        float targetVelocity;

        if (isHoldingOn)
        {
            targetVelocity = 0f;
        }
        else
        {
            targetVelocity = -playerMovement.wallSlideSpeed;


        }

        float newVel = Mathf.MoveTowards(
            player.rb.velocity.y,
            targetVelocity,
            playerMovement.wallSlideAcceleration * Time.fixedDeltaTime
            );

        player.rb.velocity = new Vector2(player.rb.velocity.x, newVel);

    }


    public override void Exit()
    {
        player.rb.gravityScale = 3f;
    }

}
