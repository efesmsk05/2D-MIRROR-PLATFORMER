using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : NetworkBehaviour, IInteractable
{
    [SyncVar]
    private Vector2 spawnPos;

    // --- YERDEYKEN KULLANILAN SENKRONÝZASYON ---
    [SyncVar(hook = nameof(OnPosChanged))]
    private Vector2 itemPos;
    [SyncVar]
    private Vector2 itemVel;

    // --- TUTULURKEN KULLANILAN SENKRONÝZASYON ---
    [SyncVar(hook = nameof(OnParentChanged))]
    private uint parentNetId = 0; // 0 = Ebeveyni yok (dünyada)

    [SyncVar(hook = nameof(OnHeldStateChanged))]
    private bool isHeld = false;

    private Rigidbody2D rb;
    private Vector2 targetPos; // Yumuþatma için hedef pozisyon

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        spawnPos = transform.position;
        itemPos = rb.position;
        targetPos = rb.position;
    }

    public override void OnStartClient()
    {
        targetPos = transform.position;
        if (!isServer)
        {
            rb.isKinematic = true;
        }
    }

    // --- POZÝSYON GÖNDERME ---
    private void FixedUpdate()
    {
        // Obje tutuluyorsa, pozisyon senkronizasyonunu DURDUR.
        // Ebeveyni olan oyuncu zaten kendi pozisyonunu senkronize ediyor.
        if (isHeld) return;

        // Obje yerdeyse ve yetki sunucudaysa, fizik pozisyonunu yolla.
        if (isServer)
        {
            itemPos = rb.position;
            itemVel = rb.velocity;
        }
    }

    // --- POZÝSYON ALMA (YUMUÞATMA) ---
    private void Update()
    {
        // Obje tutuluyorsa veya pozisyonu biz belirliyorsak (yetki bizdeyse)
        // yumuþatma yapma.
        if (isHeld || authority) return;

        // Obje yerdeyse ve biz izleyiciysek, yumuþatma yap
        ClientItemMove();
    }

    private void ClientItemMove()
    {
        // Obje SADECE YERDEYKEN süzülür (Lerp)
        Vector2 estimatedPos = targetPos + (itemVel * (float)NetworkTime.rtt);
        transform.position = Vector2.Lerp(transform.position, estimatedPos, 15f * Time.deltaTime); // Hýzý 15'e düþürdüm
    }


    // --- HOOK METODLARI (TÜM CLIENT'LARDA ÇALIÞIR) ---

    // itemPos deðiþtiðinde
    private void OnPosChanged(Vector2 oldPos, Vector2 newPos)
    {
        targetPos = newPos;
    }

    // isHeld deðiþtiðinde
    private void OnHeldStateChanged(bool oldState, bool newState)
    {
        isHeld = newState;
        // Obje tutuluyorsa fiziði kapat, býrakýlýyorsa aç
        rb.isKinematic = newState;
    }

    // parentNetId deðiþtiðinde (EN ÖNEMLÝ KISIM)
    private void OnParentChanged(uint oldId, uint newId)
    {
        parentNetId = newId;

        if (newId == 0) // Ebeveyn yoksa (býrakýldýysa)
        {
            transform.SetParent(null);
        }
        else // Yeni bir ebeveyn atandýysa (tutulduysa)
        {
            // Yeni ebeveyni (Oyuncu) netId'sinden bul
            if (NetworkClient.spawned.TryGetValue(newId, out NetworkIdentity parentIdentity))
            {
                // Oyuncudaki 'itemHoldTransform'u bul
                PlayerInteraction playerInteraction = parentIdentity.GetComponent<PlayerInteraction>();
                if (playerInteraction != null)
                {
                    // Objemizi o oyuncunun 'itemHoldTransform'una baðla
                    transform.SetParent(playerInteraction.itemHoldTransform);
                }
            }
        }
    }

    // --- PUBLIC METODLAR ---

    public void Interact(NetworkIdentity player)
    {
        Debug.Log("Item picked up by player: " + player.netId);
    }

    // PlayerInteraction bu metodu [Command] içinden çaðýrýr
    [Server]
    public void ServerSetHeldState(bool held)
    {
        isHeld = held; // Tüm client'lara yeni durumu (hook ile) bildirir
    }

    // PlayerInteraction bu metodu [Command] içinden çaðýrýr
    [Server]
    public void ServerSetParent(NetworkIdentity newParent)
    {
        if (newParent == null)
        {
            parentNetId = 0; // 0 = ebeveyn yok
        }
        else
        {
            parentNetId = newParent.netId;
        }
    }

    [Server]
    public void RespawnItem()
    {
        transform.position = spawnPos;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        itemPos = spawnPos;
        itemVel = Vector2.zero;

        // Obje respawn olduðunda tutulma durumunu ve ebeveynini sýfýrla
        isHeld = false;
        parentNetId = 0;
        rb.isKinematic = false;
    }
}