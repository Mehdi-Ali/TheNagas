using FishNet.Object;
using UnityEngine;

public class PlayerIdleState : BaseState
{

    PlayerStateManger _player;

    protected int _IdleHash;

    public void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();
        
        //caching Hashes
        _IdleHash = Animator.StringToHash("Idle");

    }
    
    
    public override void EnterState()
    {
        if(IsServer)
            _player.NetworkAnimator.CrossFade(_IdleHash, 0.15f, 0);
    }

    public override void UpdateState(){}

    public override void ExitState(){}

}
