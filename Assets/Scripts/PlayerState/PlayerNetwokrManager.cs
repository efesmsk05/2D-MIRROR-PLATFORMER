using UnityEngine;
using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;

public class PlayerNetworkManager : NetworkBehaviour
{
    [Header("State Machine")]
    private PlayerState currentState;

    [Header("Local Player Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener playerAudioListener;
    [SerializeField] public Animator animator;

    [Header("Preferences")]
    [SerializeField] public InputManager inputManager;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public PlayerInteraction playerInteraction;
    [SerializeField] public SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Transform playerBodyTransform;

    public Vector2 GetMoveDirection() => playerMovement.playerMoveDirection;
    public Rigidbody2D GetRigidbody() => rb;


    [Header("SyncVar")]
    [SyncVar] public Vector2 playerPos;
    [SyncVar] public Vector2 playerVel;
    [SyncVar] public bool isReadyToChangeScene = false;
    [SyncVar(hook = nameof(PlayerSpriteChanges))] public Vector2 playerDirection;

    public override void OnStartServer()
    {
        base.OnStartServer();
        playerPos = transform.position;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        inputManager.OnInteractPressed += playerInteraction.TryInteract;
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            // DÝKKAT: State'e artýk 'playerMovement' referansýný da veriyoruz!
            ChangeState(new IdleState(this, playerMovement));

            // Bu PlayerMovement'a taþýndý, onun Awake'inde ayarlanýyor
            // playerFacingDirection = Vector2.right; 

            if (playerCamera != null) playerCamera.enabled = true;
            if (playerAudioListener != null) playerAudioListener.enabled = true;
        }
        else
        {
            if (playerCamera != null) playerCamera.enabled = false;
            if (playerAudioListener != null) playerAudioListener.enabled = false;

            rb.isKinematic = true;
            if (playerMovement != null)
                playerMovement.enabled = false; // Fizik script'ini devre dýþý býrak
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            currentState?.Update();
            if (NetworkClient.ready)
                HandleInput();
        }
        else
        {
            ClientMove();
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            currentState?.FixedUpdate();

            if (NetworkClient.ready)
            {
                CmdMove(rb.position, rb.velocity);
            }
        }
    }

    private void HandleInput()
    {
        playerMovement.SetMoveDirection(inputManager.moveDir);

        if (playerMovement.playerMoveDirection.sqrMagnitude > .01f)
        {
            Vector2 newDir = playerMovement.playerMoveDirection.normalized;

            if (newDir.x != 0f)
            {
                playerMovement.lastFaceXDirection = new Vector2(newDir.x, 0f);
            }

            playerMovement.playerFacingDirection = newDir;

            if (newDir != playerDirection)
            {
                CmdUpdateDirection(newDir);
            }
        }
    }


    #region Client Move & Commands

    private void ClientMove()
    {
        Vector2 targetPos = playerPos + (playerVel * (float)NetworkTime.rtt);
        transform.position = Vector2.Lerp(transform.position, targetPos, 30f * Time.deltaTime);
    }


    [Command]
    private void CmdMove(Vector2 pos, Vector2 velocity)
    {
        playerPos = pos;
        playerVel = velocity;
    }

    [Command]
    private void CmdUpdateDirection(Vector2 newDir)
    {
        playerDirection = newDir;
    }

    #endregion

    #region State Machine
    public void ChangeState(PlayerState newState)
    {
        if (currentState?.GetType() == newState?.GetType()) return;
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    #endregion

    #region Other Controls

    //SYNCVAR HOOKS
    private void PlayerSpriteChanges(Vector2 oldDir, Vector2 newDir)
    {
        if (newDir.x > 0.1f) // Sað
        {
            playerBodyTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (newDir.x < -0.1f) // Sol
        {
            playerBodyTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }           
        

    #endregion // hooks

}