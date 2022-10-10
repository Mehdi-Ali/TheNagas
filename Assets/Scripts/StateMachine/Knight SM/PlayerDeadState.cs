using UnityEngine;

public class PlayerDeadState : DeadState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;


    public override void Awake()
    {
        base.Awake();

        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        _player.Animator.CrossFade(_DeadHash, 0.15f);
        _player.ReadyToSwitchState = false;

    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {

    }

}
