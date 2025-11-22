using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody2D rb;

    private IInteractable currentIInteractable;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private float raycastDistance = 1.5f;

    [Header("Item Hold Settings")]
    [SerializeField] public Transform itemHoldTransform;
    private GameObject heldItem = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #region Item Interaction 

    [Client]
    private void RaycastInteraction()
    {
        if (!isLocalPlayer) return;

        Vector2 rayDirection = playerMovement.lastFaceXDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, raycastDistance, interactableLayerMask);

        if (hit.collider != null && heldItem == null)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                Debug.Log("Raycast hit interactable: " + hit.collider.name);
                CmdHoldItem(hit.collider.gameObject);
            }
        }
        else if (heldItem != null)
        {
            ReleaseItem();
        }
    }

    //HOLD SÝDE

    [Command]
    private void CmdHoldItem(GameObject item)
    {
        NetworkIdentity itemId = item.GetComponent<NetworkIdentity>();

        if (itemId.AssignClientAuthority(connectionToClient))
        {
            Item itemScript = item.GetComponent<Item>();
            if (itemScript != null)
            {
                // 1. Objenin 'isHeld' durumunu sunucuda true yap
                itemScript.ServerSetHeldState(true);
                // 2. Objenin ebeveynini 'bu oyuncu' olarak ayarla
                itemScript.ServerSetParent(this.netIdentity);
            }

            TargetRPCHoldItem(item);
        }
    }

    [TargetRpc]
    private void TargetRPCHoldItem(GameObject item)
    {
        heldItem = item;
        heldItem.GetComponent<IInteractable>().Interact(this.netIdentity);

        heldItem.transform.SetParent(itemHoldTransform);

        heldItem.GetComponent<Collider2D>().isTrigger = true;
        heldItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    //RELEASE SÝDE

    [Client]
    private void ReleaseItem()
    {
        if (heldItem == null) return;

        const float throwForce = .1f;
        Vector2 releaseVelocity = new Vector2(playerMovement.lastFaceXDirection.x * throwForce, playerMovement.rb.velocity.y);

        CmdReleaseItem(heldItem, releaseVelocity);

        // --- Objeyi LOKAL olarak býrak ---
        Rigidbody2D itemRb = heldItem.GetComponent<Rigidbody2D>();
        Collider2D itemCollider = heldItem.GetComponent<Collider2D>();

        heldItem.transform.SetParent(null);

        if (itemCollider != null) itemCollider.isTrigger = false;

        if (itemRb != null)
        {
            itemRb.constraints = RigidbodyConstraints2D.None;
            itemRb.bodyType = RigidbodyType2D.Dynamic;
            itemRb.velocity = releaseVelocity;

            StartCoroutine(ItemConstraintControl(itemRb));
        }

        heldItem = null;
    }

    [Command]
    private void CmdReleaseItem(GameObject item, Vector2 velocity)
    {
        NetworkIdentity itemIdentity = item.GetComponent<NetworkIdentity>();

        if (itemIdentity.connectionToClient == connectionToClient)
        {
            itemIdentity.RemoveClientAuthority();

            Item itemScript = item.GetComponent<Item>();
            if (itemScript != null)
            {
                // 1. Objenin 'isHeld' durumunu sunucuda false yap
                itemScript.ServerSetHeldState(false);
                // 2. Objenin ebeveynini 'null' (yok) olarak ayarla
                itemScript.ServerSetParent(null);
            }

            item.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }


    private IEnumerator ItemConstraintControl(Rigidbody2D itemRb)
    {
        yield return new WaitForSeconds(0.2f);
        while (itemRb != null && itemRb.velocity.magnitude > .1f)
        {
            yield return new WaitForFixedUpdate();
        }

        if (itemRb != null)
        {
            itemRb.constraints = RigidbodyConstraints2D.FreezePositionX;
            itemRb.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
    }

    private void OnDrawGizmos()
    {
        if (playerMovement == null) return;
        Vector2 rayDirection = playerMovement.lastFaceXDirection;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rayDirection * raycastDistance));
    }

    #endregion


    #region Trigger Controls

    [ClientCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;
        if (collision.CompareTag("Lever") || collision.CompareTag("Item"))
        {
            currentIInteractable = collision.GetComponent<IInteractable>();
        }
    }

    [ClientCallback]
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;
        if (currentIInteractable == collision.GetComponent<IInteractable>())
        {
            currentIInteractable = null;
        }
    }

    #endregion


    #region Event Controls 

    [Client]
    public void TryInteract()// button Interaction Control
    {
        RaycastInteraction();

        if (currentIInteractable != null)
        {
            NetworkBehaviour interactableNetBehaviour = currentIInteractable as NetworkBehaviour;
            if (interactableNetBehaviour != null)
            {
                CmdInteract(interactableNetBehaviour.netIdentity);
            }
        }
    }

    [Command]
    private void CmdInteract(NetworkIdentity interactableIdentity)
    {
        IInteractable interactable = interactableIdentity.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this.netIdentity);
        }
    }

    #endregion
}