using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuperAttackState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    //Variables to store optimized Setter / getter parameter IDs
    int _SuperAttackHash;


    // utilities 
    private int _attackCounter ;

    public void Awake()
    {

        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _SuperAttackHash = Animator.StringToHash("SuperAttack");


        _attackCounter = 0 ;
  
    }
    public override void EnterState()
    {
        Debug.Log("Supper Attack !!") ;
        _enemy.Animator.CrossFade(_SuperAttackHash, 0.15f);
        _enemy.ReadyToSwitchState = false ;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        _enemy.ReadyToSwitchState = true ;
    }

}
