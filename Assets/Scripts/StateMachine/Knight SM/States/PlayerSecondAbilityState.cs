using System.Net.Mime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSecondAbilityState : BaseState, IHasCooldown
{
    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store optimized Setter / getter parameter IDs
    int _secondAbilityHash;
    int _secondAbilityMultiplierHash ;

    // cooldown things
    public string Id => _player.Statics.SecondAbilityAbilityName;
    public float CooldownDuration => _player.Statics.SecondAbilityCooldown;
    public Image Image => _player.CooldownUIManager.CooldownUI2.Image;

    private void Awake()
    {       
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbilityMultiplierHash = Animator.StringToHash("SecondAbility_Multiplier");
       
    }

    public override void EnterState()
    {
        _player.Animator.SetFloat(_secondAbilityMultiplierHash, _player.Statics.SecondAbilityAnimationSpeed);
        _player.CooldownSystem.PutOnCooldown(this);

        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _player.Statics.SecondAbilityAnimationSpeed);
                
        _player.NetworkAnimator.CrossFade(_secondAbilityHash, 0.1f, 0);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;
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
        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(_player.Statics.SecondAbilityDamage);
        }

        _player.ActiveAttackCollider.Collider.enabled = false ;
    }






}
