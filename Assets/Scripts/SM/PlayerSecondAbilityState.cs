using UnityEngine;

public class PlayerSecondAbilityState : PlayerBaseState
{
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _SecondAbilityHash;

    private void Awake()
    {
        //caching Hashes
        _SecondAbilityHash = Animator.StringToHash("SecondAbility");
    }

    public override void EnterState(PlayerStateManger player)
    {
        if (!base.IsOwner) return;
        Invoke(nameof(AttackComplete), 1f);
        this.player = player;
        player.NetworkAnimator.SetTrigger(_SecondAbilityHash);
        player.ReadyToSwitchState = false;
    }

    public override void UpdateState(PlayerStateManger player)
    {

    }

    public override void ExitState(PlayerStateManger player)
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        player.ReadyToSwitchState = true;
        player.IsStationary = false ;
        player.SwitchState(player.IdleState);
    }
}
