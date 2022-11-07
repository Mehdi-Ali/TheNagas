using System.Collections.Generic;
using UnityEngine;

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
        var animSpeed = _player.Statics.ThirdAbilityAnimationSpeed ;
        var animDuration = _player.AnimationsLength.ThirdAbilityDuration * 0.90f;

        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackSemiComplete), animDuration / animSpeed);
        Invoke(nameof(AttackSemiComplete), animDuration / animSpeed);
        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, animSpeed);

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
        _tCoeff = animSpeed / animDuration;
        _start = transform.position;
        _end = _player.TargetPosition;

    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
        base.OnTickState();
        DealDamage();
        if (_player.NeedsMoveAndRotate) return;
        FirstPartOfDashing();

    }

    private void DealDamage()
    {

        foreach (EnemyBase enemy in _player.HitBoxes.Targets)
        {
            if (!enemy.IsAlive) continue;
            if (_damagedTargets.Contains(enemy))
                continue;

            enemy.TakeDamage(_player.Statics.ThirdAbilityDamage);

            _damagedTargets.Add(enemy);

        }

        _player.HitBoxes.Targets.ExceptWith(_damagedTargets);
    }

    private void FirstPartOfDashing()
    {
        _tLerp += (float)base.TimeManager.TickDelta * _tCoeff;

        transform.position = Vector3.Lerp(_start, _end, _tLerp);
    }

    public override void ExitState(){}

    void AttackSemiComplete()
    {
        var animSpeed = _player.Statics.ThirdAbilityAnimationSpeed ;
        var animDuration = _player.AnimationsLength.ThirdAbilityDuration * 0.1f;

        _player.NeedsMoveAndRotate = true;

        var directionVector = (_end - _start).normalized;
        _player.SetMoveData(directionVector.x, directionVector.z);
        

        Invoke(nameof(AttackComplete), animDuration / animSpeed);
    }

    private void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
        _player.ActiveAttackCollider.Collider.enabled = false;
        _player.IsAutoAiming = false;
        _player.NeedsMoveAndRotate = false;
    }
}
