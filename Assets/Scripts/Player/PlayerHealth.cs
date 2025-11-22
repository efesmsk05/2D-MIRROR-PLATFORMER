using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private PlayerNetworkManager player;
    [SerializeField] private PlayerMovement playerMovement;



    [SyncVar] public Vector2 respawnPosition;


    // events 
    public static event Action<PlayerNetworkManager> OnPlayerDied;
    public static event Action<PlayerNetworkManager> OnPlayerRespawn;


    #region Death Process

    public void Die()
    {
        if (!isServer) return;
        StartCoroutine(ServerSideDied());
    }

    public IEnumerator ServerSideDied()
    {
        RpcPlayerDied();
        yield return new WaitForSeconds(.35f);

        player.rb.position = respawnPosition;
        player.playerPos = respawnPosition;
        player.playerVel = Vector2.zero;

        TargetRpcTeleport(respawnPosition);
        yield return new WaitForSeconds(.1f);
        RpcPlayerRespawned();
    }

    [TargetRpc]
    private void TargetRpcTeleport(Vector2 newPosition)
    {
        playerMovement.rb.position = newPosition;
        playerMovement.rb.velocity = Vector2.zero;
    }

    [ClientRpc]
    private void RpcPlayerDied()
    {
        OnPlayerDied?.Invoke(player);
        if (isLocalPlayer)
        {
            player.ChangeState(new PlayerDiedState(player, playerMovement));
        }
    }

    [ClientRpc]
    public void RpcPlayerRespawned()
    {
        OnPlayerRespawn?.Invoke(player);
    }

    #endregion

    public void ServerCheckPoint(Vector2 currentSpawnPoint)
    {
        respawnPosition = currentSpawnPoint;
    }



}
