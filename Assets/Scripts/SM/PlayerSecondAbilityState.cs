using System.Collections.Generic;
using UnityEngine;

public class PlayerSecondAbilityState : PlayerBaseState, IHasCooldown
{
    //Name of The Ability
    public string AbilityName = "Slash";

    //Game Design Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 2.0f;
    [SerializeField] float _cooldown = 5.0f;
    [SerializeField] float _attackRang = 1.5f;
    [SerializeField] float _damage = 20.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _secondAbilityHash;
    int _secondAbilityMultiplierHash ;

    // cooldown things
    public string Id => AbilityName;
    public float CooldownDuration => _cooldown;

    private void Awake()
    {       
         //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbilityMultiplierHash = Animator.StringToHash("SecondAbility_Multiplier");
       
       _player.Animator.SetFloat(_secondAbilityMultiplierHash, _animationSpeed);
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        _player.CooldownSystem.PutOnCooldown(this);

        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _animationSpeed);
                
        _player.Animator.CrossFade(_secondAbilityHash, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false ;
        _player.SwitchState(_player.IdleState);
    }

    void SecondAbilityStartEvent()
    {
        _player.HitBoxes.Targets.Clear();
       _player.ActiveAttackCollider.Collider.enabled = true ;

    }

    void SecondAbilityEndEvent()
    {       

        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(_damage);
        }

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = false ;

    }





}
