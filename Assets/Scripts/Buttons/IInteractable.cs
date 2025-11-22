using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IInteractable
{
    public void Interact(NetworkIdentity player);
}
