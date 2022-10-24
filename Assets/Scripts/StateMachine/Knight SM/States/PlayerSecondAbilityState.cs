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

    #region Only Client Vars ----------------------------------------------
    #if !UNITY_SERVER

    //Variables to store optimized Setter / getter parameter IDs
    int _secondAbilityHash;
    int _secondAbilityMultiplierHash ;

    #endif
    #endregion

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _player = GetComponent<PlayerStateManger>();

        if (!Owner.IsLocalClient) return ;
        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbilityMultiplierHash = Animator.StringToHash("SecondAbility_Multiplier");

        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUI2.Image);
    }


    public override void EnterState()
    {
        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _player.Statics.SecondAbilityAnimationSpeed);

        _player.NetworkAnimator.CrossFade(_secondAbilityHash, 0.1f, 0);
                
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        if (!IsOwner) return;
        _player.Animator.SetFloat(_secondAbilityMultiplierHash, _player.Statics.SecondAbilityAnimationSpeed);


        _player.HitBoxes.Targets.Clear();
        _player.ActiveAttackCollider.Collider.enabled = true ;
    }

    [ServerRpc(RunLocally = true)]
    private void RpcEnterState()
    {
    }

    public override void UpdateState() {}

    public override void ExitState() {}

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
