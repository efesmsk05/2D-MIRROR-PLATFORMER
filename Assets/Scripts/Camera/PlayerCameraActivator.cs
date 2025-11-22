using Mirror;
using UnityEngine;

// Bu script Player prefab'ýnýn ROOT objesinde (NetworkIdentity olan yerde) olmalý
public class PlayerCameraActivator : NetworkBehaviour
{
    [Header("Kamera Bileþenleri")]
    [SerializeField]
    private Camera playerCamera; // Inspector'dan 'Camera' child objesini sürükle

    [SerializeField]
    private AudioListener audioListener; // Inspector'dan 'Camera' objesindeki AudioListener'ý sürükle

    [SerializeField]
    private PlayerCameraSetup playerCam; // 'Camera' objesindeki PlayerCameraSetup script'ini sürükle

    [Header("Takip Hedefi")]
    [SerializeField]
    private Transform cameraFollowTarget; // 'Player_Body' objesini buraya sürükle


    // --- YENÝ EKLENEN KISIM ---
    // Bu metod, 'OnStartLocalPlayer'dan HEMEN ÖNCE çalýþýr
    public override void OnStartClient()
    {
        // Baþlangýçta, bu prefab'ýn (kime ait olursa olsun) kamerasýný kapat.
        // Bu, prefab'da kapalý unutsan bile garantili bir kapatma saðlar.
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        if (audioListener != null)
        {
            audioListener.enabled = false;
        }

        if (playerCam != null)
        {
            playerCam.enabled = false;
        }
    }
    // --- YENÝ EKLENEN KISIM SONU ---


    public override void OnStartLocalPlayer()
    {
        // Bu kod SADECE bu bilgisayarýn kontrol ettiði oyuncu için çalýþýr.
        // 'OnStartClient'te her þeyi kapattýðýmýz için, burada SADECE
        // yerel oyuncuya ait olanlarý GÜVENLE açabiliriz.

        Debug.Log("OnStartLocalPlayer çaðrýldý. Kamera AÇILIYOR.");

        // 1. Kamera ve Ses Dinleyiciyi AÇ
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }

        if (audioListener != null)
        {
            audioListener.enabled = true;
        }

        // 2. Kameraya KÝMÝ takip edeceðini SÖYLE
        if (playerCam != null && cameraFollowTarget != null)
        {
            playerCam.enabled = true; // Script'in kendisini de aç
            playerCam.SetTargetTransform(cameraFollowTarget);
        }
        else
        {
            Debug.LogError("Kamera script'i (playerCam) veya takip hedefi (cameraFollowTarget) atanmamýþ!");
        }
    }
}