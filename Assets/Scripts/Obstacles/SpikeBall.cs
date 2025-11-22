using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpikeBall : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return; // Sadece sunucu tarafýnda çalýþtýr

        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Die();
            }
        }


    }
}
