using UnityEngine;
using Mirror;

public class OnOffButton : NetworkBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject target;

    [SyncVar(hook = nameof(OnChangeIsPressed))]
    public bool isPressed = false;

    public void Interact(NetworkIdentity player)
    {
        ServerToggleButton();
    }

    [Server]
    private void ServerToggleButton()
    {
        IActivatable activatable = target.GetComponent<IActivatable>();

        if (activatable != null && activatable.isBusy())
        {
            return; 
        }

        isPressed = !isPressed;

        if (activatable != null)
        {
            activatable.Activate(isPressed);
        }
    }

    // hooks
    private void OnChangeIsPressed(bool oldValue, bool newValue)
    {
        sr.color = newValue ? Color.green : Color.red;
    }
}