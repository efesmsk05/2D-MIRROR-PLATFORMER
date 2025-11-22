using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTimer;
    private float originalGravity;

    private float dashDamping = 0.91f;

    public PlayerDashState(PlayerNetworkManager player , PlayerMovement playerMovement ) : base(player , playerMovement)
    {
    }

    public override void Enter()
    {

        player.animator.SetBool("isDashing", true);


        dashTimer = playerMovement.dashDuration;

        originalGravity = player.rb.gravityScale;
        player.rb.gravityScale = 0f; // Yer�ekimini kapat


        player.rb.velocity = player.playerDirection * playerMovement.dashForce;


    }

    public override void Update()
    {
        playerMovement.IsBrokeable();

        dashTimer -= Time.deltaTime;
        
        if (playerMovement.IsTouchingWall() && !playerMovement.IsGrounded())
        {
            player.ChangeState(new PlayerWallState(player , playerMovement));

            return;
        }

        if (dashTimer <= 0f)
        {

            if (playerMovement.IsTouchingWall() && !playerMovement.IsGrounded())
            {
                //player.animator.SetBool("isDashing", false);

                player.ChangeState(new PlayerWallState(player  , playerMovement));

                return;
            }

            if (player.inputManager.isMoving)
            {
                //player.animator.SetBool("isDashing", false);

                player.ChangeState(new WalkState(player , playerMovement));
            }
            else if( !playerMovement.IsGrounded())
            {
                player.ChangeState(new PlayerFallState(player , playerMovement));
            }
            else
            {
                //player.animator.SetBool("isDashing", false);

                player.ChangeState(new IdleState(player , playerMovement));
            }
        }
 
    }

    public override void FixedUpdate()
    {

        player.rb.velocity *= dashDamping;
    }

    public override void Exit()
    {

        player.rb.gravityScale = originalGravity;


        if (!player.inputManager.isMoving && playerMovement.IsGrounded())
        {
            playerMovement.rb.velocity = Vector2.zero;
        }
        else if (!player.inputManager.isMoving && !playerMovement.IsGrounded())
        {
            return;
        }
    }
}