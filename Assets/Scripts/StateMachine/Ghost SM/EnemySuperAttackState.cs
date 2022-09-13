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

    public void Start()
    {
        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _superAttackHash = Animator.StringToHash("SuperAttack");
        _superAttackMultiplierHash = Animator.StringToHash("SuperAttack__Multiplier");

        _enemy.Animator.SetFloat(_superAttackMultiplierHash, _enemy.Statics.SuperAttackSpeed);
    }

    public override void EnterState()
    {
        Invoke(nameof(AttackComplete), _enemy.AnimationsLength.SuperAttack_Duration / _enemy.Statics.SuperAttackSpeed );

        _enemy.Animator.CrossFade(_superAttackHash, 0.15f);
        _enemy.ReadyToSwitchState = false ;
    }

    public override void UpdateState()
    {
        _direction = (_enemy.Player.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _enemy.Statics.SuperAttackRotationSpeed);
    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }
}
