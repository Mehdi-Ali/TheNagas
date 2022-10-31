using System.Net.Mime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class PlayerSecondAbilityState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id =>  OwnerId + _player.Statics.SecondAbilityAbilityName;
    public float CooldownDuration => _player.Statics.SecondAbilityCooldown;

    int _secondAbilityHash;
    int _secondAbilityMultiplierHash ;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();

        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbilityMultiplierHash = Animator.StringToHash("SecondAbility_Multiplier");

        if (!Owner.IsLocalClient) return ;
        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUI2.Image);
    }


    public override void EnterState()
    {
        
        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _player.Statics.SecondAbilityAnimationSpeed);
        _player.Animator.SetFloat(_secondAbilityMultiplierHash, _player.Statics.SecondAbilityAnimationSpeed);

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;


        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_secondAbilityHash, 0.1f, 0);
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true;
        }
    }

    public override void UpdateState() {}

    public override void ExitState() {}

    [Server]
    void SecondAbilityStartEvent()
    {
        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            enemy.TakeDamage(_player.Statics.SecondAbilityDamage);
        }

        _player.ActiveAttackCollider.Collider.enabled = false ;
    }
    
    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false ;
        _player.SwitchState(_player.IdleState);
    }







}
