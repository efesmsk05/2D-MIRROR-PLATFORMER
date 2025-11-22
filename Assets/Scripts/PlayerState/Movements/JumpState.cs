using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerNetworkManager player , PlayerMovement playerMovement) : base(player , playerMovement) { }

    public override void Enter()
    {
        player.animator.SetBool("isJumping", true);


        playerMovement.SetNormalGravity();
        playerMovement.PlayerJump();
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
            player.animator.SetBool("isJumping", false);

            return;
        }

        if (playerMovement.IsTouchingWall() && !playerMovement.IsGrounded())
        {
            player.ChangeState(new PlayerWallState(player , playerMovement));
            player.animator.SetBool("isJumping", false);

            return;
        }

        if (player.rb.velocity.y < 0.1f)
        {
            player.ChangeState(new PlayerFallState(player , playerMovement));
            return;
        }


        if (player.inputManager.IsJumpCutoff() && player.rb.velocity.y > 0f)
        {
            playerMovement.SetFastFallGravity();
        }
        else if (!player.inputManager.IsJumpCutoff() && Mathf.Abs(player.rb.velocity.y) < playerMovement.jumpApexVelocityThreshold)
        {
            playerMovement.SetJumpApexGravity();
        }
        else
        {
            playerMovement.SetNormalGravity();
        }
    }


}