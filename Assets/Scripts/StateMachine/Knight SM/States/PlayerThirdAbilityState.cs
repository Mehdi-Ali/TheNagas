using System.Collections.Generic;
using UnityEngine;

public class PlayerThirdAbilityState : BaseState, IHasCooldown
{
    PlayerStateManger _player;

    public string Id => _player.Statics.ThirdAbilityAbilityName;
    public float CooldownDuration => _player.Statics.ThirdAbilityCooldown;

    int _thirdAbilityHash;
    int _thirdAbilityMultiplierHash ;

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
        var animDuration = _player.AnimationsLength.ThirdAbilityDuration;

        _player.CooldownSystem.PutOnCooldown(this);
        Invoke(nameof(AttackComplete), animDuration / animSpeed);
        _player.Animator.SetFloat(_thirdAbilityMultiplierHash, animSpeed);

        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
        _player.NeedsMoveAndRotate = true;


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
        var jumpTime = animDuration / animSpeed;
        var start = transform.position ;
        var end = _player.TargetPosition ;

        var speed = (start - end).magnitude / jumpTime;
        var directionVector = (end - start).normalized;

        _player.SetMoveAndRotateSpeed(speed, 0f);
        _player.SetMoveData(directionVector.x, directionVector.z);

        // ! this part is diff
        Physics.IgnoreLayerCollision(6, 7);
    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
        base.OnTickState();
        DealDamage();
        if (_player.NeedsMoveAndRotate) return;
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

    private void ThirdAbilityEndEvent()
    {
        Physics.IgnoreLayerCollision(6, 7, false);
    }

    public override void ExitState(){}

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
