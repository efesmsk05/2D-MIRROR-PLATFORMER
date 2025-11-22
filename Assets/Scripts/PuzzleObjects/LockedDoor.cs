using UnityEngine;
using Mirror; 
using System.Collections;

public class LockedDoor : NetworkBehaviour, IActivatable
{
    [SyncVar]
    private bool isMoving = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    [SerializeField] private float moveSpeed = 20f;

    private Coroutine movementCoroutine;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, 2f, 0);
    }


    [Server]
    public void Activate(bool activateState)
    {
        if (isMoving) return;

        isMoving = true;

        Vector3 targetPosition = activateState ? openPosition : closedPosition;

        // 1. Tüm client'lara kapýyý hareket ettirmeleri için komut yolla
        RpcMoveDoor(targetPosition);

        // 2. Sunucu da kapýnýn ne zaman duracaðýný bilmek için
        //    hareket simülasyonunu (ve kilidi açmayý) baþlatýr.
        StartCoroutine(ServerMoveTimer(targetPosition));
    }

    [Server]
    public bool isBusy()
    {
        return isMoving; 
    }

    // --- Sunucu Taraflý Mantýk ---

    [Server]
    private IEnumerator ServerMoveTimer(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

        
        isMoving = false;
    }

    // --- Client Taraflý Mantýk ---

    [ClientRpc] 
    private void RpcMoveDoor(Vector3 targetPosition)
    {
        // Eðer önceki hareket bitmemiþse onu durdur
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        // Client üzerinde yumuþak hareketi baþlat
        movementCoroutine = StartCoroutine(MoveDoor(targetPosition));
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        movementCoroutine = null;
    }


}