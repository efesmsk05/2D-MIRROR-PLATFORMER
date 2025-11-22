using UnityEngine;

public abstract class PlayerState
{
    // State'lerin ana Player objesine ve bileþenlerine eriþebilmesi için
    // protected bir referans tutuyoruz.
    protected PlayerNetworkManager player;
    protected PlayerMovement playerMovement;
    // Constructor'ý private yerine protected yapýyoruz.
    protected PlayerState(PlayerNetworkManager player , PlayerMovement playerMovement)
    {
        this.player = player;
        this.playerMovement = playerMovement;
    }

    // Bu metotlar sanal (virtual) kalacak, her state kendi içinde dolduracak.
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}