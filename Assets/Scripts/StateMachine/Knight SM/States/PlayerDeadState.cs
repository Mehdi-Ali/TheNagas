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
        _player.NetworkAnimator.CrossFade(_DeadHash, 0.15f, 0);
        _player.ReadyToSwitchState = false;

    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {

    }

}
