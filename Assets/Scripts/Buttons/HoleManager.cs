using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : NetworkBehaviour
{



    public GameObject HolePrefab = null;
    public Transform HoleSpawnPoint = null;

    [Server]
    public void OpenHole()
    {
        Hole();
    }

    private void Hole()
    {
        if (HolePrefab != null)
        {
            var hole = Instantiate(HolePrefab, HoleSpawnPoint.transform.position, Quaternion.identity);
            NetworkServer.Spawn(hole);
        }
        else
        {
            Debug.LogWarning("[Server] HolePrefab is missing!");
        }
    }
}
