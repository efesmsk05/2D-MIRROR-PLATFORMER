using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{


    private Vector2 JumpVel;

    private float exitTimer;
    public PlayerWallJumpState(PlayerNetworkManager player , Vector2 jumpVelocity , PlayerMovement playerMovement) : base(player , playerMovement)
    {
        this.JumpVel = jumpVelocity;
    }


    public override void Enter()
    {
        player.rb.velocity = JumpVel;

        player.rb.gravityScale = 3f;

        exitTimer = playerMovement.wallJumpExitTime;


    }

    public override void Update()
    {
        exitTimer -= Time.deltaTime;

        if (exitTimer <= 0f )
        {
            player.ChangeState(new PlayerFallState(player , playerMovement));
            return;
        }

    }

    public override void FixedUpdate()
    {
        if (playerMovement.IsGrounded())
        {
            player.ChangeState(new IdleState(player , playerMovement));
        }
    }

}
