using UnityEngine;

public class PlayerFirstAbilityState : PlayerBaseState
{
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _firstAbility;

    private void Awake()
    {
        //Caching The Player State Manger
        player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _firstAbility = Animator.StringToHash("FirstAbility");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), 1f);
        player.NetworkAnimator.SetTrigger(_firstAbility);
        player.ReadyToSwitchState = false;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        player.ReadyToSwitchState = true;
        player.IsCastingAnAbility = false;
        player.SwitchState(player.IdleState);
    }
}
