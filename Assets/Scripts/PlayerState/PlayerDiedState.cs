using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PlayerDiedState : PlayerState
{
    public PlayerDiedState(PlayerNetworkManager player , PlayerMovement playerMovement) : base(player , playerMovement)
    {
    }

    public override void Enter()
    {
        player.StartCoroutine(DeadCoroutine());
    }


    private IEnumerator DeadCoroutine()
    {
        yield return EffectManager.Instance.FadeScreen(false , .35f);

        yield return new WaitForSeconds(.2f);

        yield return EffectManager.Instance.FadeScreen(true, .35f);

        player.ChangeState(new IdleState(player, playerMovement));

    }




}
