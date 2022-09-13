using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicAttackState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    //Variables to store optimized Setter / getter parameter IDs
    int _BasicAttackHash;
    int _BasicAttackMultiplierHash;


    // utilities 
    private int _attackCounter ;

    public void Awake()
    {

        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _BasicAttackHash = Animator.StringToHash("BasicAttack");
        _BasicAttackMultiplierHash = Animator.StringToHash("BasicAttack__Multiplier");


        _attackCounter = 0 ;
  
    }
    public override void EnterState()
    {
        if (_attackCounter == 2)
        {
            _enemy.SwitchState(_enemy.SuperAttackState);
            _attackCounter = 0 ;
        }
        else 
        {
            Debug.Log("BasicAttack");
            _enemy.Animator.CrossFade(_BasicAttackHash, 0.15f);
            _enemy.ReadyToSwitchState = false ;

            _attackCounter++ ;
        }

    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        _enemy.ReadyToSwitchState = true ;
    }

}
