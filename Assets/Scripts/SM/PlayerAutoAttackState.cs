using UnityEngine;

public class PlayerAutoAttackState : PlayerBaseState
{
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _autoAttack;

    private void Awake()
    {
        //Caching The Player State Manger
        player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _autoAttack = Animator.StringToHash("AutoAttack");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), 1f);
        player.NetworkAnimator.SetTrigger(_autoAttack);
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
