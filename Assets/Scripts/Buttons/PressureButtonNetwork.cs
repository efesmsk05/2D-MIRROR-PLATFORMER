using UnityEngine;
using Mirror;

public class PressureButtonNetwork : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnActiveStateChanged))]
    public bool isActive = false;

    [Header("Target")]
    [SerializeField] private GameObject target;

    [SerializeField]private SpriteRenderer currentSpriteRenderer;
    [SerializeField]private Sprite pressSprite;
    [SerializeField] private Sprite normalSprite;


    private void Awake()
    {
       currentSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;

        if (collision.CompareTag("Player") || collision.CompareTag("Block"))
        {
            isActive = true;

            IActivatable activatable = target.GetComponent<IActivatable>();
            if (activatable != null)
            {
                activatable.Activate(isActive);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isServer) return;

        if (collision.CompareTag("Player") || collision.CompareTag("Block"))
        {
            isActive = false;

            IActivatable activatable = target.GetComponent<IActivatable>();
            if (activatable != null)
            {
                activatable.Activate(isActive);
            }
        }
    }

    void OnActiveStateChanged(bool oldVal, bool newVal)
    {
        currentSpriteRenderer.sprite = newVal ? pressSprite : normalSprite;
    }
}
