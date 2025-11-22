using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckPointer : NetworkBehaviour
{
    [SerializeField] private  SpriteRenderer flagSprite;

    private Color originalColor = Color.white;

    [SyncVar(hook = nameof(FlagColorChanger))]
    bool isActivated = false;

    private void Start()
    {
        flagSprite.color = originalColor;
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        PlayerHealth player = collision.GetComponent<PlayerHealth>();

        if (player != null )
        {
            player.ServerCheckPoint(this.transform.position);

            isActivated = true;

        }

    }

    private void FlagColorChanger(bool oldVal , bool newVal)
    {

        if (newVal)
        {
            flagSprite.color = Color.green;
        }
        else
        {
            flagSprite.color = originalColor;
        }

    }


}
