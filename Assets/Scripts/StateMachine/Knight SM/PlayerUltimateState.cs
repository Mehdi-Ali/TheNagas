using System.Collections.Generic;
using UnityEngine;

public class PlayerUltimateState : BaseState, IHasCooldown
{
    //Variables...
    bool _grounded ;
    float _tLerp ;
    Vector3 _end;
    Vector3 _start;


    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _ultimateHash;
    int _ultimateMultiplierHash;

    // cooldown things
    public string Id => _player.Statics.UltimateAbilityAbilityName;
    public float CooldownDuration => _player.Statics.UltimateAbilityCooldown;


    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _ultimateHash = Animator.StringToHash("Ultimate");
        _ultimateMultiplierHash = Animator.StringToHash("Ultimate_Multiplier");

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        _player.CooldownSystem.PutOnCooldown(this);

        _grounded = false;
        _tLerp = 0.0f ;
        _start = transform.position ;
        _end = _player.HitBoxes.transform.position ;


        Invoke( nameof(AttackComplete),
                (_player.AnimationsLength.UltimateDuration / _player.Statics.UltimateAbilityAnimationSpeed ));
        
        _player.Animator.SetFloat(_ultimateMultiplierHash, _player.Statics.UltimateAbilityAnimationSpeed);
        _player.Animator.CrossFade(_ultimateHash, 0.1f);


        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;
    }

    public override void UpdateState()
    {
  
        if (_grounded) return;
        _tLerp += Time.deltaTime * _player.Statics.UltimateAbilityAnimationSpeed / ( _player.AnimationsLength.UltimateDuration - ((41f - 28f) / 30f ));
        transform.position = Vector3.Lerp( _start, _end, _tLerp );

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

    void UltimateStartEvent()
    {
        _grounded = true ;
    }

    void UltimateEndEvent()
    {
        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(_player.Statics.UltimateAbilityDamage);
        }

        _player.ActiveAttackCollider.Collider.enabled = false ;
    }



}
