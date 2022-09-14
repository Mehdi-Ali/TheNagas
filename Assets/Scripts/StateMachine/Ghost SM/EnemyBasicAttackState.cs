using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicAttackState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    // Cashing Instances


    //Variables to store optimized Setter / getter parameter IDs
    int _basicAttackHash;
    int _basicAttackMultiplierHash;


    // utilities 
    private int _attackCounter ;
    private Vector3 _direction ;
    private Quaternion _lookRotation ;

    public void Start()
    {

        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _basicAttackHash = Animator.StringToHash("BasicAttack");
        _basicAttackMultiplierHash = Animator.StringToHash("BasicAttack__Multiplier");

        _enemy.Animator.SetFloat(_basicAttackMultiplierHash, _enemy.Statics.BasicAttackSpeed);
        _attackCounter = 0 ;

    }
    public override void EnterState()
    {

        if (_attackCounter < 2)
        {
            //
            _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(true);
            //_enemy.HitBoxes.BasicCollider.Collider.enabled = true ; // we don't have any 
            //

            Invoke(nameof(AttackComplete), _enemy.AnimationsLength.BasicAttack_Duration / _enemy.Statics.BasicAttackSpeed );


            _enemy.Animator.CrossFade(_basicAttackHash, 0.15f);
            _enemy.ReadyToSwitchState = false ;

            _attackCounter++ ;
        }
        else if (_attackCounter == 2)
        {
            _enemy.SwitchState(_enemy.SuperAttackState);
            _attackCounter = 0 ;
        }

    }

    public override void UpdateState()
    {
        _direction = (_enemy.Player.transform.position - this.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _enemy.Statics.BasicAttackRotationSpeed);
    }

    public override void ExitState()
    {
        _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(false); // delete
    }

    void AttackComplete()
    {
        _enemy.ReadyToSwitchState = true ;
        _enemy.SwitchState(_enemy.IdleState);
    }

    void BasicAttackEvent()
    {
        _enemy.HitBoxes.BasicHitBox.gameObject.SetActive(false);

        //Do Damage or SHoot or slow or whatever

        //_enemy.HitBoxes.BasicCollider.Collider.enabled = false ; // we don't have any 
    }

}
