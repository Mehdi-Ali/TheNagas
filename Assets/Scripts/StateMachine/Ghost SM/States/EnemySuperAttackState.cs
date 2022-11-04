using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemySuperAttackState : BaseState
{    
    EnemyStateManger _enemy;

    int _superAttackHash;
    int _superAttackMultiplierHash;
    private Vector3 _direction ;
    private Quaternion _lookRotation ;
    private bool _doLookAt ;

    private void Awake() 
    {
        _enemy = GetComponent<EnemyStateManger>();
        
        _superAttackHash = Animator.StringToHash("SuperAttack");
        _superAttackMultiplierHash = Animator.StringToHash("SuperAttack__Multiplier");
    }

    public override void EnterState()
    {
        _enemy.Animator.SetFloat(_superAttackMultiplierHash, _enemy.Statics.SuperAttackAnimationSpeed);

         RpcSetSuperHitBox();
        _enemy.RpcHitBoxDisplay(true);
        _enemy.HitBoxes.SuperCollider.Collider.enabled = true;

        Invoke(nameof(AttackComplete), _enemy.AnimationsLength.SuperAttack_Duration / _enemy.Statics.SuperAttackAnimationSpeed );

        _enemy.NetworkAnimator.CrossFade(_superAttackHash, 0.15f, 0);
        _enemy.ReadyToSwitchState = false ;
        _doLookAt = true;
    }

    [ObserversRpc]
    private void RpcSetSuperHitBox()
    {
        _enemy.ActiveHitBox = _enemy.HitBoxes.SuperHitBox;
    }
    
    public override void OnTickState()
    {
        base.OnTickState();
        Rotate();
    }

    private void Rotate()
    {
        if (!_doLookAt) return;
        _direction = (_enemy.TargetPlayer.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        var rotationSpeed = (float)TimeManager.TickDelta * _enemy.Statics.SuperAttackRotationSpeed;
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, rotationSpeed);  
    }
    public override void UpdateState(){}

    public override void ExitState(){}

    [Server]
    void SuperAttackEvent()
    {
        _doLookAt = false;
        
        _enemy.RpcHitBoxDisplay(false);

        foreach(PlayerBase player in _enemy.HitBoxes.Targets)
        {
            player.TakeDamage(_enemy.Statics.SuperAttackDamage);
        }

        _enemy.HitBoxes.SuperCollider.Collider.enabled = false ;
    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }
}
