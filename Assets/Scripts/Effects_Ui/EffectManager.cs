using System.Collections;
using UnityEngine;
using System; // Action için
using Mirror;
using Unity.IO.LowLevel.Unsafe; // NetworkBehaviour için

public class EffectManager : NetworkBehaviour
{
    public static EffectManager Instance  { get; private set; }

    [Header("Referances")]
    [SerializeField] private GameObject player;
    [SerializeField] private CanvasGroup blackScreenPanel;

    public override void OnStartClient()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
        PlayerHealth.OnPlayerRespawn += HandlePlayerRespawn;

    }

    private void HandlePlayerDied(PlayerNetworkManager manager)
    {
        // Bu event herkes için tetiklenir, ama sadece doðru oyuncu için iþlem yaparýz.
            // sprite kapatýcaz
        manager.playerSpriteRenderer.enabled = false;

            //efects
            //sound 
            //bunlar herkes tarafýndan görülür

    }

    private void HandlePlayerRespawn(PlayerNetworkManager manager)
    {

        manager.playerSpriteRenderer.enabled = true;
        
    }


    public override void OnStopClient()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
        PlayerHealth.OnPlayerRespawn -= HandlePlayerRespawn;
    }



    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public IEnumerator FadeScreen(bool fadeIn , float duration) // fade in 1 veya 0 olucak (true veya false) ekran kapansýnmý açýlsýnmý deðeridir
    {

        float startAlpha = fadeIn ? 1f : 0f; 
        float endAlpha = fadeIn ? 0f : 1f;

        float timer = 0f;


        while (timer < duration)
        {
            timer += Time.deltaTime;
            blackScreenPanel.alpha = Mathf.Lerp(startAlpha , endAlpha , timer / duration);
            yield return null; // bir frame bekle

        }
        blackScreenPanel.alpha = endAlpha; 

    }

}
