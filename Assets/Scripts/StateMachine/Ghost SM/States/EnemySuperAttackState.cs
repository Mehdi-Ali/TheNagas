using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuperAttackState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    //Variables to store optimized Setter / getter parameter IDs
    int _superAttackHash;
    int _superAttackMultiplierHash;
    private Vector3 _direction ;
    private Quaternion _lookRotation ;


    // utilities 
    private bool _doLookAt ;

    public void Start()
    {
        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _superAttackHash = Animator.StringToHash("SuperAttack");
        _superAttackMultiplierHash = Animator.StringToHash("SuperAttack__Multiplier");
        
    }

    public override void EnterState()
    {
        _enemy.Animator.SetFloat(_superAttackMultiplierHash, _enemy.Statics.SuperAttackAnimationSpeed);
        _enemy.HitBoxes.SuperHitBox.gameObject.SetActive(true);
        _enemy.HitBoxes.SuperCollider.Collider.enabled = true ;
        Invoke(nameof(AttackComplete), _enemy.AnimationsLength.SuperAttack_Duration / _enemy.Statics.SuperAttackAnimationSpeed );

        _enemy.NetworkAnimator.CrossFade(_superAttackHash, 0.15f, 0);
        _enemy.ReadyToSwitchState = false ;
        _doLookAt = true;
    }

    public override void UpdateState()
    {
        // if (!_doLookAt) return;
        // _direction = (_enemy.TargetPlayer.transform.position - this.transform.position).normalized;
        // _lookRotation = Quaternion.LookRotation(_direction);
        // transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _enemy.Statics.SuperAttackRotationSpeed);
       
    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }

    void SuperAttackEvent()
    {
        _doLookAt = false;
        
        _enemy.HitBoxes.SuperHitBox.gameObject.SetActive(false);

        foreach(PlayerBase player in _enemy.HitBoxes.Targets)
        {
            player.TakeDamage(_enemy.Statics.SuperAttackDamage);
        }


        _enemy.HitBoxes.SuperCollider.Collider.enabled = false ;
    }
}
