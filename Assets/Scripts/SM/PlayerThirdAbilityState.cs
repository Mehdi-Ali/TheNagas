using UnityEngine;

public class PlayerThirdAbilityState : PlayerBaseState
{
    //Ability Name
    public string AbilityName = "Dash" ;

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] float _dashStpeed = 20.0f;
    [SerializeField] float _animationSpeed = 2f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _thirdAbilityHash;
    int _thirdAbilityMultiplierHash ;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _thirdAbilityHash = Animator.StringToHash("ThirdAbility");
        _thirdAbilityMultiplierHash = Animator.StringToHash("ThirdAbility_Multiplier");

        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, _animationSpeed);

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), _player.AnimationsLength.ThirdAbilityDuration /_animationSpeed );
        _player.Animator.CrossFade(_thirdAbilityHash,0.1f);
        //activating colider
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {
        transform.Translate(Vector3.forward * _dashStpeed * Time.deltaTime);

    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }
}
