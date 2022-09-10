using System.Collections.Generic;
using UnityEngine;

public class PlayerThirdAbilityState : BaseState, IHasCooldown
{
    //Ability Name
    public string AbilityName = "Dash" ;

    //Game Design Vars, Mak a stat Script maybe
    [SerializeField] float _animationSpeed = 2f;
    [SerializeField] float _cooldown = 5.0f;
    [SerializeField] float _damage = 20.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables...
    float _tLerp;
    Vector3 _end;
    Vector3 _start;
    public HashSet<EnemyBase> _targetsToRemove = new HashSet<EnemyBase>();

    //Variables to store optimized Setter / getter parameter IDs
    int _thirdAbilityHash;
    int _thirdAbilityMultiplierHash ;

    // cooldown things
    public string Id => AbilityName;
    public float CooldownDuration => _cooldown;

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
        _player.CooldownSystem.PutOnCooldown(this);

        _tLerp = 0.0f;
        _start = transform.position;
        _end = _player.ActiveHitBox.transform.position;

        Invoke(nameof(AttackComplete), _player.AnimationsLength.ThirdAbilityDuration /_animationSpeed );
        _player.Animator.CrossFade(_thirdAbilityHash,0.1f);

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {
        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, _animationSpeed);

        _tLerp += Time.deltaTime * _animationSpeed / _player.AnimationsLength.ThirdAbilityDuration;
        transform.position = Vector3.Lerp(_start, _end, _tLerp);

        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(_damage);

            _targetsToRemove.Add(enemy);
        }

        _player.HitBoxes.Targets.ExceptWith(_targetsToRemove);
            



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

        _player.ActiveAttackCollider.Collider.enabled = false ; 
    }
}
