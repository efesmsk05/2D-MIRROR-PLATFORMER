using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoutineButton : NetworkBehaviour , IInteractable
{
    [SyncVar] public bool isPressed = false;
    [SyncVar] public bool routineActive = false;

    public SpriteRenderer sprite;
    public void Interact(NetworkIdentity player)
    {
        if (isServer)
        {
            ServerPressButton();
        }
        else
        {
            CmdPressButton();
        }
    }

    [Command]
    private void CmdPressButton()
    {
        ServerPressButton();
    }

    [Server]
    private void ServerPressButton()
    {
        if (isPressed)
        {
            Debug.Log("Button already pressed!");
            return;
        }
        isPressed = true;

        if(!routineActive && isPressed)
        {
            StartCoroutine(Routien());

        }

    }

    private IEnumerator Routien()
    {
        RpcRoutine();

        routineActive = true;

        Debug.Log("Routine started!");

        yield return new WaitForSeconds(1f);
        RpcRoutine();

        yield return new WaitForSeconds(1f);
        RpcRoutine();

        yield return new WaitForSeconds(1f);

        Debug.Log("Routine finished!");

        routineActive = false;

        yield return new WaitForSeconds(.2f);

        RpcRoutine();

        isPressed = false;

    }

    [ClientRpc]
    private void RpcRoutine()
    {
      

        if (isPressed && routineActive)
        {
            sprite.color = Random.ColorHSV();
        }
        else if (!routineActive)
        {
            sprite.color = Color.red;
        }

    }



}
