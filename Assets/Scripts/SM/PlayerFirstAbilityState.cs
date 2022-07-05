using UnityEngine;

public class PlayerFirstAbilityState : PlayerBaseState
{
    //Name of The Abbility
    public string AbbilityName = "Spin" ;

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _movementSpeed = 7.5f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

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
        //activating colider
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {
        if (!base.IsOwner) return;
        _player.Move(_movementSpeed);
    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }
}
