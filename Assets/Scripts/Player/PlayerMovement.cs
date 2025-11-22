using UnityEngine;
using Mirror; // Mirror'a ihtiyacý yok, kaldýrýlabilir
using System.Collections;
using System.Collections.Generic;

// Bu script artýk NetworkBehaviour deðil, normal bir MonoBehaviour
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;

    [Header("Player Movement")]
    [SerializeField] public float moveSpeed = 20f;

    //JUMP
    [SerializeField] public float jumpForce = 10f;
    [SerializeField] public float originalGravityScale = 3f;
    [SerializeField] private float fastFallGravityScale = 5f;

    [Tooltip("Zýplamanýn zirvesindeyken kullanýlacak düþük yerçekimi (asma hissi için)")]
    [SerializeField] private float jumpApexGravityScale = 1.5f; // Normalin yarýsý
    [Tooltip("Hýz bu deðerin altýna düþtüðünde 'zirve' olarak kabul edilecek")]
    [SerializeField] public float jumpApexVelocityThreshold = 2.5f;

    //DASH
    [SerializeField] public float dashForce = 25;
    [SerializeField] public float dashDuration = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Wall Check")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask brokeableLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallCheckBoxSize;

    [Header("Wall Mechanics")]
    [SerializeField] public float wallSlideSpeed = 0.5f; // Duvarda kayma hýzý
    [SerializeField] public float wallSlideAcceleration = 50f; // Kayma hýzýna ulaþma ivmesi
    [SerializeField] public float wallJumpHorizontalForce = 7f; // Duvardan zýplama yatay gücü
    [SerializeField] public float wallJumpExitTime = 0.15f;

    // Girdi ve Yön
    [HideInInspector] public Vector2 playerMoveDirection;
    [HideInInspector] public Vector2 lastFaceXDirection;
    [HideInInspector] public Vector2 playerFacingDirection;

    private void Awake()
    {
        // Script baþladýðýnda varsayýlan yerçekimini ayarla
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale;
        }
        playerFacingDirection = Vector2.right;
    }

    // NetworkManager bu metodu çaðýrarak hareket yönünü belirleyecek
    public void SetMoveDirection(Vector2 direction)
    {
        playerMoveDirection = direction;
    }

    #region Movement Logic

    public void PlayerMove()
    {
        float targetVelocity = playerMoveDirection.x * moveSpeed;

        float newVelocityX = Mathf.MoveTowards(
            rb.velocity.x,          // Þu anki hýzým
            targetVelocity,         // Hedef hýzým
            100 * Time.fixedDeltaTime // Ývmelenme
        );

        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }

    public void PlayerJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void PlayerJumpCutoff()
    {
        if (rb.velocity.y > 0f)
        {
            float targetYVelocity = rb.velocity.y * .5f;
            float newvelocityY = Mathf.MoveTowards(
                rb.velocity.y,
                targetYVelocity,
                100 * Time.fixedDeltaTime
            );
            rb.velocity = new Vector2(rb.velocity.x, newvelocityY);
        }
    }

    #endregion

    #region Check Logic

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    public bool IsTouchingWall()
    {
        Collider2D collider = Physics2D.OverlapBox(
            wallCheckPoint.position,  // Kutunun merkezi
            wallCheckBoxSize,         // Kutunun boyutlarý
            0f,                       // Açý
            wallLayer                 // Layer
        );
        return collider != null;
    }

    public bool IsBrokeable()
    {
        Vector2 origin = wallCheckPoint.position;
        Vector2 direction = playerFacingDirection.normalized; // Bu deðiþkeni de buraya taþýdýk
        float distance = wallCheckBoxSize.x;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, brokeableLayer);

        if (hit.collider != null)
        {
            var brokeable = hit.collider.GetComponent<IBrokeable>();
            if (brokeable != null)
            {
                brokeable.Break();
            }
        }
        return hit.collider != null;
    }

    #endregion

    #region Gravity Logic

    public void SetNormalGravity()
    {
        rb.gravityScale = originalGravityScale;
    }

    public void SetFastFallGravity()
    {
        rb.gravityScale = fastFallGravityScale;
    }

    public void SetJumpApexGravity()
    {
        rb.gravityScale = jumpApexGravityScale;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        // Zemin kontrolü
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        // Duvar kontrolü
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheckPoint.position, wallCheckBoxSize);
        }
    }

    #endregion
}