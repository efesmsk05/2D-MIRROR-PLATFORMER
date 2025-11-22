using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BigObstacleWall : NetworkBehaviour, IActivatable
{
    [SyncVar]
    private bool isMoving = false;

    [SyncVar(hook = nameof(OnTargetPositionChanged))]
    private Vector2 targetPos;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    [SerializeField] private float moveSpeed = .1f;

    private Coroutine serverMovementCoroutine;
    private Coroutine clientMovementCoroutine;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector2(0, 3.5f);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        targetPos = closedPosition; 
    }

    [Server]
    public void Activate(bool activetaState)
    {

        targetPos = activetaState ? openPosition : closedPosition;

        if (serverMovementCoroutine != null) StopCoroutine(serverMovementCoroutine);
        serverMovementCoroutine = StartCoroutine(ServerMoveWall(targetPos));
    }

    public bool isBusy()
    {
        return isMoving;
    }

    [Server]
    private IEnumerator ServerMoveWall(Vector2 newTargetPos)
    {
        isMoving = true; 

        while (Vector2.Distance(transform.position, newTargetPos) > 0.01f)
        {
            float step = moveSpeed * Time.fixedDeltaTime;
            transform.position = Vector2.MoveTowards(transform.position, newTargetPos, step);
            yield return new WaitForFixedUpdate();
        }

        transform.position = newTargetPos;
        isMoving = false; 
        serverMovementCoroutine = null;
    }

    // === CLIENT TARAFI ===
    //hook fonksiyonu
    private void OnTargetPositionChanged(Vector2 oldPos, Vector2 newPos)
    {
        if (isServer) return;

        if (clientMovementCoroutine != null)
        {
            StopCoroutine(clientMovementCoroutine);
        }
        clientMovementCoroutine = StartCoroutine(ClientMoveWall(newPos));
    }

   // for clients
    private IEnumerator ClientMoveWall(Vector2 newTargetPos)
    {
        while (Vector2.Distance(transform.position, newTargetPos) > 0.01f)
        {
            float step = moveSpeed * Time.fixedDeltaTime;
            transform.position = Vector2.MoveTowards(transform.position, newTargetPos, step);
            yield return new WaitForFixedUpdate();
        }
        transform.position = newTargetPos;
        clientMovementCoroutine = null;
    }
}