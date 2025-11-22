using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeableWall : MonoBehaviour , IBrokeable
{
    public void Break()
    {
        print("Wall Broken!");

        Destroy(gameObject);
    }
}
