using FishNet.Object;
using UnityEngine;

public class PlayerIdleState : BaseState
{
    //A reference for the Player State Manger
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    protected int _Idle;

    public void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();
        
        //caching Hashes
        _Idle = Animator.StringToHash("Idle");

    }
    
    [Client(RequireOwnership = true)]
    public override void EnterState()
    {
        _player.NetworkAnimator.CrossFade(_Idle, 0.15f, 0);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

}
