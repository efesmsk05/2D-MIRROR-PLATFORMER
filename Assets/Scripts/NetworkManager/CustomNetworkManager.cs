using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Bunu ekle

public class CustomNetworkManager : NetworkManager
{
    public List<Transform> spawnPoints = new List<Transform>();
    private int nextSpawnPointIndex = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        FindSpawnPoints();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        FindSpawnPoints();
    }

    private void FindSpawnPoints()
    {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>()
                        .Select(pos => pos.transform)
                        .ToList();

        nextSpawnPointIndex = 0;

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Bu sahnede hiç 'NetworkStartPosition' bileþeni bulunamadý.");
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Spawn noktasý listesi boþsa, varsayýlan metodu çaðýr
        if (spawnPoints.Count == 0)
        {
            base.OnServerAddPlayer(conn);
            return;
        }

        // Spawn noktasý kalmadýysa döngüyü baþa sar
        if (nextSpawnPointIndex >= spawnPoints.Count)
        {
            nextSpawnPointIndex = 0;
        }

        Transform startPos = spawnPoints[nextSpawnPointIndex];
        nextSpawnPointIndex++;

        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        // Oyuncuya kendi baþlangýç pozisyonunu ata
        PlayerHealth playerController = player.GetComponent<PlayerHealth>();
        if (playerController != null)
        {
            playerController.respawnPosition = startPos.position;
        }
    }
}