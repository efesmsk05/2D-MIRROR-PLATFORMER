using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System; // Input System için gerekli

public class InputManager : NetworkBehaviour
{
    private PlayerControls controls;
    public Vector2 moveDir { get; private set; }
    public bool isMoving { get; private set; }
    
    public bool jumpKeyHeld { get; private set; }
    public bool jumpKeyReleased { get; private set; }

    public bool isDashPressed { get; private set; }
    public bool isDashOnCooldown = false;

    public bool interactKeyPressed { get; private set; }



    [Header("Game Feel Settings")]
    [SerializeField] private float jumpBufferTime = 1f; 
    private float jumpBufferTimer;
    [SerializeField] private float dashResetTime = 2f;


    [SerializeField] private float maxJumpHoldTime = 0.4f; 
    public float jumpHoldTimer { get; private set; }

    // Events
    public event Action OnInteractPressed;
        



    // ------------------------------------

    private void Awake()
    {
        controls = new PlayerControls();
        isDashOnCooldown = false;
        isDashPressed = false;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (jumpKeyHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }

    }

    public override void OnStartLocalPlayer()
    {
        controls.PlayerMovement.Enable();
        controls.PlayerMovement.Horizontal.performed += OnMove;
        controls.PlayerMovement.Horizontal.canceled += OnMove;

        controls.PlayerMovement.Vertical.started += OnJump; 
        controls.PlayerMovement.Vertical.canceled += OnJumpDisable;

        controls.PlayerMovement.Dash.started += OnDash;

        controls.PlayerMovement.Interact.started += ctx => { OnInteractPressed?.Invoke(); };

    }

    private void OnDisable()
    {
        if (!isLocalPlayer) return;
        controls.PlayerMovement.Disable();
        controls.PlayerMovement.Horizontal.performed -= OnMove;
        controls.PlayerMovement.Horizontal.canceled -= OnMove;
        controls.PlayerMovement.Vertical.started -= OnJump;
        controls.PlayerMovement.Vertical.canceled -= OnJumpDisable;
        controls.PlayerMovement.Dash.started -= OnDash;

        controls.PlayerMovement.Interact.started += ctx => { OnInteractPressed?.Invoke(); };

    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector2>();
        isMoving = moveDir.x != 0f;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        // DEĞİŞİKLİK: 'playerIsJumping = true' yerine zamanlayıcıyı başlatıyoruz.
        jumpBufferTimer = jumpBufferTime;

        jumpKeyHeld = true;
        jumpKeyReleased = false;

        jumpHoldTimer = 0f; 
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if(!isDashOnCooldown)
        {
            isDashPressed = true;
            StartCoroutine(DashCooldownCoroutine());
        }
;
    }

    private IEnumerator DashCooldownCoroutine()
    {
        isDashOnCooldown = true;

        yield return new WaitForSeconds(dashResetTime);

        isDashOnCooldown = false;

    }

    public void DashReset()
    {
        isDashPressed = false;
    }


    private void OnJumpDisable(InputAction.CallbackContext ctx)
    {
        jumpKeyHeld = false;
        jumpKeyReleased = true;
    }


    public bool IsJumpBuffered()
    {
        return jumpBufferTimer > 0f;
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferTimer = 0f;
    }

    public bool IsJumpCutoff()
    {
        // tuş bırakıldı veya maksimum tutma süresi aşıldıysa true döner
        return jumpKeyReleased || jumpHoldTimer > maxJumpHoldTime;

    }
}