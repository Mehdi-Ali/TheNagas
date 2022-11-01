using FishNet.Object;
using UnityEngine;

public class PlayerIdleState : BaseState
{

    PlayerStateManger _player;

    int _IdleHash;

    public void Awake()
    {
        _player = GetComponent<PlayerStateManger>();
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
