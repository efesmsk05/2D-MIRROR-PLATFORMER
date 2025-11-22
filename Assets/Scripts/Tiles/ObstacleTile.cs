using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Obstacle Tile", menuName = "Tiles/Obstacle Tile")]
public class ObstacleTile : TileBase
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // Temel görsel veriyi (sprite, renk vb.) buradan alabilirsiniz.
        // Örneðin: tileData.sprite = someSprite;
        // ...
    }

    // Bu tile'ý kullanan bir Tilemap Collider 2D bileþeni varsa,
    // çarpýþma tipini buradan ayarlayabilirsiniz.


}
