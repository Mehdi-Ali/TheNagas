using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class PlayerThirdAbilityState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.ThirdAbilityAbilityName;
    public float CooldownDuration => _player.Statics.ThirdAbilityCooldown;

    int _thirdAbilityHash;
    int _thirdAbilityMultiplierHash ;

    float _tLerp;
    float _tCoeff;
    Vector3 _end;
    Vector3 _start;
    public HashSet<EnemyBase> _damagedTargets = new HashSet<EnemyBase>();


    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _player = GetComponent<PlayerStateManger>();

        _thirdAbilityHash = Animator.StringToHash("ThirdAbility");
        _thirdAbilityMultiplierHash = Animator.StringToHash("ThirdAbility_Multiplier");

        if (!Owner.IsLocalClient) return ;
        _player.CooldownSystem.ImageDictionary.Add(Id,_player.CooldownUIManager.CooldownUI3.Image);
    }

    public override void EnterState()
    {

        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackComplete), _player.AnimationsLength.ThirdAbilityDuration /_player.Statics.ThirdAbilityAnimationSpeed );
        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, _player.Statics.ThirdAbilityAnimationSpeed);

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;

        if (IsServer)
        {   
            _player.NetworkAnimator.CrossFade(_thirdAbilityHash,0.1f, 0);   
            _player.HitBoxes.Targets.Clear();

            _player.HitBoxes.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _player.ActiveAttackCollider.Collider.enabled = true ;

            // ! this part is diff
            _damagedTargets.Clear();
        }

        // ! this part is diff
        _tLerp = 0.0f;
        _tCoeff = _player.Statics.ThirdAbilityAnimationSpeed / _player.AnimationsLength.ThirdAbilityDuration ;
        _start = transform.position;
        _end = _player.TargetPosition;

    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
        base.OnTickState();
        ThirdAbilityStartEvent();
    }

    private void ThirdAbilityStartEvent()
    {
        _tLerp += (float)base.TimeManager.TickDelta * _tCoeff ;

        transform.position = Vector3.Lerp(_start, _end, _tLerp);

        foreach(EnemyBase enemy in _player.HitBoxes.Targets)
        {
            if (!enemy.IsAlive) continue;
            if (_damagedTargets.Contains(enemy))
                continue;
            
            enemy.TakeDamage(_player.Statics.ThirdAbilityDamage);

            _damagedTargets.Add(enemy);

        }

        _player.HitBoxes.Targets.ExceptWith(_damagedTargets);
    }

    public override void ExitState(){}

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false ; 
    }
}
