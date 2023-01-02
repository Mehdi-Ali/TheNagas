using UnityEngine;

public class PlayerDeadState : BaseState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    protected int _DeadHash;


    public void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _DeadHash = Animator.StringToHash("Dead");

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
