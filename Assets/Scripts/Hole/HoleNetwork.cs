using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HoleNetwork : NetworkBehaviour
{

    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //    if (!isServer) return; //only server check

    //    if (collision.CompareTag("Player"))
    //    {
    //        // Burada oyuncuyu yok etme veya baþka bir iþlem yapabilirsiniz

    //        var player = collision.GetComponent<PlayerNetwork>();

    //        if (player != null)
    //        {
    //            player.isReadyToChangeScene = true;

    //            CheckAllPlayersReady();

    //            HidePlayers(player.netId);

    //        }
    //    }
    //}



    //private void CheckAllPlayersReady()
    //{
    //    // Þu anda baðlý olan tüm player objelerini Mirror üzerinden al
    //    bool allReady = true;

    //    foreach (var conplayers in NetworkServer.connections)
    //    {
    //        if (conplayers.Value != null && conplayers.Value.identity != null)
    //        {
    //            var player = conplayers.Value.identity.GetComponent<PlayerNetwork>();
    //            if (player == null || !player.isReadyToChangeScene)
    //            {
    //                allReady = false;



    //                break;
    //            }
    //        }
    //    }

    //    if (allReady)
    //    {
    //        SceneManagerNetwork.instance.ChangeScene("Game_Scene1");
    //    }
    //    else
    //    {
    //        Debug.Log("[Server] Not all players are ready yet.");
    //    }
    //}

    //[ClientRpc]
    //private void HidePlayers(uint player)
    //{
    //    if (NetworkClient.spawned.TryGetValue(player, out NetworkIdentity ni)) // girilen butonun Identity'sini alýyoruz
    //    {
    //         ni.gameObject.SetActive(false);
    //    }
    //}
}
