using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ButtonUiManager : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [System.Serializable]
    public class ButtonUi
    {
        public int buttonId;

        public Animator animator;

        public AudioClip audioClip;

        public SpriteRenderer spriteRenderer;

    }

    public ButtonUi[] buttons;

    //private void OnEnable()
    //{
    //    ButtonNetwork.OnButtonPressed += ButtonNetwork_ButtonUiChange;
    //}

    //private void OnDisable()
    //{
    //    ButtonNetwork.OnButtonPressed -= ButtonNetwork_ButtonUiChange;
    //}


    private void ButtonNetwork_ButtonUiChange(int buttonid)
    {

        foreach (var btn in buttons)
        {
            if (btn.buttonId == buttonid)
            {
                //if(btn.audioSource != null) btn.audioSource.Play();

                //if(btn.animator != null) btn.animator.SetTrigger("Pressed");

                Debug.Log($"[Client] Button {buttonid} pressed. Playing UI effects.");

                if (btn.spriteRenderer != null) btn.spriteRenderer.color = Color.blue;


            }
        }
    }

  


}
