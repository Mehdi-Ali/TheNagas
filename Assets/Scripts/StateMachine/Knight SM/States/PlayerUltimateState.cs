using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUltimateState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.UltimateAbilityAbilityName;
    public float CooldownDuration => _player.Statics.UltimateAbilityCooldown;

    int _ultimateHash;
    int _ultimateMultiplierHash;



    float _tLerp ;
    float _tCoeff ;
    Vector3 _end;
    Vector3 _start;


    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();
        
        _ultimateHash = Animator.StringToHash("Ultimate");
        _ultimateMultiplierHash = Animator.StringToHash("Ultimate_Multiplier");

        if (!Owner.IsLocalClient) return ;
        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUIU.Image);
    }

    public override void EnterState()
    {

        _player.CooldownSystem.PutOnCooldown(this);
        Invoke( nameof(AttackComplete),(_player.AnimationsLength.UltimateDuration / _player.Statics.UltimateAbilityAnimationSpeed ));
        _player.Animator.SetFloat(_ultimateMultiplierHash, _player.Statics.UltimateAbilityAnimationSpeed);
        
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
        _player.NeedsMoveAndRotate = true;
        
        if (IsServer)
        {
            _player.NetworkAnimator.CrossFade(_ultimateHash, 0.1f, 0);
        _player.HitBoxes.Targets.Clear();

        _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player.ActiveAttackCollider.Collider.enabled = true ;

        }

        // ! this part is diff
        _tLerp = 0.0f ;
        _tCoeff = _player.Statics.UltimateAbilityAnimationSpeed / ( _player.AnimationsLength.UltimateDuration - ((41f - 28f) / 30f ));
        _start = transform.position ;
        _end = _player.TargetPosition ;

        // new 

        var speed = (_start - _end).magnitude * _tCoeff ;
        var directionVector = (_end - _start).normalized;
        _player.SetMoveAndRotateSpeed(speed, 0f);
        _player.SetMoveData(directionVector.x, directionVector.z);
    }

    public override void UpdateState(){}
    public override void ExitState(){}

    void UltimateStartEvent()
    {
        _player.NeedsMoveAndRotate = false;
    }

    void UltimateEndEvent()
    {
        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            if (!enemy.IsAlive) continue;
            enemy.TakeDamage(_player.Statics.UltimateAbilityDamage);
        }
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ; 
        _player.IsAutoAiming = false ;
    }

}
