using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 

public class SceneChanger : NetworkBehaviour
{

    public string sceneName;
    [SerializeField] private SpriteRenderer openedDoor;
    [SerializeField] private  SpriteRenderer currentDoorSprite;

    [SyncVar (hook = nameof(ChangeDoorSprite))]
    bool isActive = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;

        if (collision.CompareTag("Player"))
        {

            PlayerNetworkManager player = collision.GetComponent<PlayerNetworkManager>();

            if (player != null)
            {
                player.isReadyToChangeScene = true;
                
                isActive = true;

                CheckAllPlayersReady();

            }

        }


    }


    private void CheckAllPlayersReady()
    {
        if (NetworkServer.connections.Count == 0) return;// Sunucuda kimse yoksa çýk

        bool allReady = true;

        foreach (var readyPlayers in NetworkServer.connections)
        {
            NetworkConnectionToClient conClient = readyPlayers.Value;

            if (conClient == null || conClient.identity == null)
            {
                allReady = false;
                break;
            }

            PlayerNetworkManager player = conClient.identity.GetComponent<PlayerNetworkManager>();

            if (player == null || !player.isReadyToChangeScene)
            {
                allReady = false; // Oyuncu var ama hazýr deðil
                break;
            }

        }

        if (allReady)
        {
            foreach(var players in NetworkServer.connections)
            {
                PlayerNetworkManager player = players.Value.identity.GetComponent<PlayerNetworkManager>();

                if (player != null)
                {
                    player.isReadyToChangeScene = false; // Hazýr durumunu sýfýrla
                }
            }

            SceneManagerNetwork.instance.ChangeScene(sceneName);
        }
        else
        {
            Debug.Log("[Server] Not all players are ready yet.");
        }


    }

    private void ChangeDoorSprite(bool oldValue , bool newValue)
    {
        if (newValue)
        {
            currentDoorSprite.sprite = openedDoor.sprite;
        }
        
    }


}
