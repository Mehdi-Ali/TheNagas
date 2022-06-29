using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    //Variables to store omptimized Setter / getter parameter IDs
    int _DeadHash;

    private void Awake()
    {
        //caching Hashes
        _DeadHash = Animator.StringToHash("Dead");
    }

    public override void EnterState(PlayerStateManger player)
    {
        if (!base.IsOwner) return;
        player.Animator.SetTrigger(_DeadHash);
        player.ReadyToSwitchState = false;

    }

    public override void UpdateState(PlayerStateManger player)
    {
        
    }

    public override void ExitState(PlayerStateManger player)
    {

    }

}
