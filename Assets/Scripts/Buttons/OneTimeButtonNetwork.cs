using UnityEngine;
using Mirror;

public class OneTimeButtonNetwork : NetworkBehaviour, IInteractable
{
    [SyncVar(hook = nameof(OnPressedStateChanged))]
    public bool isPressed = false;

    public SpriteRenderer sr;

    void Awake()
    {
        sr.color = Color.red;  // varsayýlan renk
    }
        
    public void Interact(NetworkIdentity player)
    {
        if (isServer)
            ServerPressButton();
        else
            CmdPressButton();
    }

    [Command]
    void CmdPressButton()
    {
        ServerPressButton();
    }

    [Server]
    void ServerPressButton()
    {
        if (isPressed) return;    // tekrar basýlamaz
        isPressed = true;         // hook tetiklenecek
    }

    void OnPressedStateChanged(bool oldValue, bool newValue)
    {
        sr.color = newValue ? Color.green : Color.red;
    }
}
