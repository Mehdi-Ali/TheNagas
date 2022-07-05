using UnityEngine;

public class PlayerThirdAbilityState : PlayerBaseState
{
    //Ability Name
    public string AbilityName = "Dash" ;

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] float _animationSpeed = 2f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables...
    float _tLerp;
    Vector3 _end;
    Vector3 _start;

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
        _tLerp = 0.0f;
        _start = transform.position;
        _end = _player.ActiveHitBox.transform.position;

        Invoke(nameof(AttackComplete), _player.AnimationsLength.ThirdAbilityDuration /_animationSpeed );
        _player.Animator.CrossFade(_thirdAbilityHash,0.1f);

        //activating colider
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {
        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, _animationSpeed);

        _tLerp += Time.deltaTime * _animationSpeed / _player.AnimationsLength.ThirdAbilityDuration;
        transform.position = Vector3.Lerp(_start, _end, _tLerp);

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
