using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TileMapObstacleDetector : NetworkBehaviour
{
    public Tilemap tilemap;


    // Bu metod, baþka bir Collider bu Tilemap'in trigger'ýna girdiðinde çalýþýr.
    void OnTriggerEnter2D(Collider2D other)
    {

        if (!isServer) return;
        // Giren nesnenin "Player" olup olmadýðýný kontrol edin
        if (other.CompareTag("Player"))
        {
            // 1. Temas noktasýný dünya koordinatlarýndan Tilemap hücre koordinatlarýna dönüþtürün.
            // Temas noktasýný almak için Collider'ýn merkezini kullanmak en kolay yoldur.
            Vector3 worldPosition = other.bounds.center;
            Vector3Int tilePosition = tilemap.WorldToCell(worldPosition);

            // 2. Bu pozisyonda hangi Tile olduðunu alýn.
            TileBase tile = tilemap.GetTile(tilePosition);

            // 3. Tile'ýn bir tehlike Tile'ý olup olmadýðýný kontrol edin (yani HazardTile)
            if (tile is ObstacleTile obstacleTile)
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.Die();
                }


            }
        }
    }


}
