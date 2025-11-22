using Mirror.Examples.Basic;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerNetworkManager player, PlayerMovement playerMovement) : base(player, playerMovement) { }

    public override void Enter()
    {
        player.animator.SetBool("isIdle", true);

    }


    public override void Update()
    {
        if (player.inputManager.IsJumpBuffered() && playerMovement.IsGrounded())
        {
            player.inputManager.ConsumeJumpBuffer();
            player.ChangeState(new JumpState(player , playerMovement));
            return;


        }

        if (player.inputManager.isDashPressed)
        {
            player.inputManager.DashReset();
            player.ChangeState(new PlayerDashState(player , playerMovement));

            return;
        }


        if (player.inputManager.isMoving)
        {
            // yatayda haraket var ise 
            player.ChangeState(new WalkState(player , playerMovement));
            return;

        }


    }

    public override void FixedUpdate()
    {
        float targetVelocity = 0f ;


        float newVelocityX = Mathf.MoveTowards(
            player.rb.velocity.x,          // Þu anki hýzým (örn: dash'ten kalan 25)
            targetVelocity,         // Hedef hýzým (örn: 10)
            100 * Time.fixedDeltaTime // Saniyede 'acceleration' birim hýzla deðiþ
        );

        player.rb.velocity = new Vector2(newVelocityX, player.rb.velocity.y);


    }

    public override void Exit()
    {
        player.animator.SetBool("isIdle", false);
    }


}
