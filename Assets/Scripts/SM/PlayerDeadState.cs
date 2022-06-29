using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    //A refrence for the Player State Manger
    PlayerStateManger player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _DeadHash;

    private void Awake()
    {
        //Caching The Player State Manger
        player = GetComponent<PlayerStateManger>();
        //caching Hashes
        _DeadHash = Animator.StringToHash("Dead");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        player.NetworkAnimator.SetTrigger(_DeadHash);
        player.ReadyToSwitchState = false;

    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {

    }

}
