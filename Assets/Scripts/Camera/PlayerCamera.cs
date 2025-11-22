using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraSetup : NetworkBehaviour // NetworkBehaviour olmalý
{
    [Header("Takip Ayarlarý")]
    public Transform followTarget; // Player_Body buraya atanacak

    [Tooltip("Kameranýn takip yumuþaklýðý. 1 = anýnda, 0.1 = yavaþ")]
    [Range(0.01f, 1.0f)]
    [SerializeField] private float smoothSpeed = 0.125f;

    [SerializeField] private float fixedYPosition = 0f;
    [SerializeField] private float cameraZOffset = -10f;

    [Header("Sýnýr Ayarlarý (Confiner Yerine)")]
    [SerializeField] private bool enableXBounds = true;
    [SerializeField] private Vector2 xBounds = new Vector2(-10f, 10f);

    // --- (SAFE ZONE) ---
    [Header("Safe Zone Ayarlarý")]
    [SerializeField] private bool enableSafeZone = true;
    [Tooltip("Kameranýn hareket etmeyeceði alanýn geniþliði (Unity birimi)")]
    [SerializeField] private float safeZoneWidth = 2.0f;

    // Kameranýn takip ettiði mevcut hedef X pozisyonu
    private float currentTargetX;

    void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }

        // --- SAFE ZONE MANTIÐI ---
        if (enableSafeZone)
        {
            float playerX = followTarget.position.x;
            float safeZoneHalfWidth = safeZoneWidth / 2.0f;

            float deltaX = playerX - currentTargetX;

            if (deltaX > safeZoneHalfWidth)
            {
                currentTargetX = playerX - safeZoneHalfWidth;
            }
            else if (deltaX < -safeZoneHalfWidth)
            {
                currentTargetX = playerX + safeZoneHalfWidth;
            }
        }
        else
        {
            currentTargetX = followTarget.position.x;
        }
        // --- SAFE ZONE MANTIÐI SONU ---

        // 1. Hedef Pozisyonu Hesapla
        Vector3 desiredPosition = new Vector3(
            currentTargetX,
            fixedYPosition,
            cameraZOffset
        );

        // 2. X Sýnýrlarýný Uygula
        if (enableXBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, xBounds.x, xBounds.y);
            currentTargetX = desiredPosition.x;
        }

        // 3. Süzülme (Yumuþak Takip)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // PlayerCameraActivator tarafýndan çaðrýlacak metod
    public void SetTargetTransform(Transform target)
    {
        followTarget = target;

        if (followTarget != null)
        {
            // Baþlangýç hedefi oyuncunun X'idir
            currentTargetX = followTarget.position.x;

            // Baþlangýç pozisyonunu hesapla
            Vector3 startPosition = new Vector3(
                currentTargetX,
                fixedYPosition,
                cameraZOffset
            );

            // Baþlangýç pozisyonunu sýnýrlar içinde tut
            if (enableXBounds)
            {
                startPosition.x = Mathf.Clamp(startPosition.x, xBounds.x, xBounds.y);
                currentTargetX = startPosition.x;
            }

            // Kamerayý anýnda bu pozisyona ayarla
            transform.position = startPosition;
        }
    }

    // Sýnýrlarý debug etmek için Gizmo çiz
    void OnDrawGizmos()
    {
        // X Sýnýrlarýný (Mavi) çiz
        if (enableXBounds)
        {
            Gizmos.color = Color.cyan;
            // Camera.main null olabileceði için kontrol ekliyoruz
            float camHeight = 20f; // Varsayýlan yükseklik
            if (Camera.main != null)
            {
                camHeight = Camera.main.orthographicSize * 2;
            }

            Gizmos.DrawLine(new Vector3(xBounds.x, fixedYPosition + (camHeight / 2)), new Vector3(xBounds.x, fixedYPosition - (camHeight / 2)));
            Gizmos.DrawLine(new Vector3(xBounds.y, fixedYPosition + (camHeight / 2)), new Vector3(xBounds.y, fixedYPosition - (camHeight / 2)));
        }

        // Safe Zone'u (Kýrmýzý) çiz
        if (enableSafeZone)
        {
            Gizmos.color = Color.red;
            float halfWidth = safeZoneWidth / 2.0f;
            float zoneCenterX = currentTargetX;

            Vector3 leftLineTop = new Vector3(zoneCenterX - halfWidth, fixedYPosition + 5f, 0);
            Vector3 leftLineBottom = new Vector3(zoneCenterX - halfWidth, fixedYPosition - 5f, 0);

            Vector3 rightLineTop = new Vector3(zoneCenterX + halfWidth, fixedYPosition + 5f, 0);
            Vector3 rightLineBottom = new Vector3(zoneCenterX + halfWidth, fixedYPosition - 5f, 0);

            Gizmos.DrawLine(leftLineTop, leftLineBottom);
            Gizmos.DrawLine(rightLineTop, rightLineBottom);
        }
    }
}