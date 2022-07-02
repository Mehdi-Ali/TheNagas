using UnityEngine;

public class PlayerFirstAbilityState : PlayerBaseState
{
    //Game Designe Vars, Mak a stat Script maybe
    float _animationDuration = 0.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;
    

    //Name of The Abbility
    public string AbbilityName = "Spin" ;

    //Variables to store omptimized Setter / getter parameter IDs
    int _firstAbility;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _firstAbility = Animator.StringToHash("FirstAbility");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), _player.AnimationsLength.FirstAbilityDuration);
        _player.Animator.CrossFade(_firstAbility, 0.1f);
        _player.ReadyToSwitchState = false;
    }

    public override void UpdateState()
    {
        //move with x Speed
    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        //_player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }
}
