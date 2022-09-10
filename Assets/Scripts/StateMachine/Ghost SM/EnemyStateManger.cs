using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManger : StateManger
{

    //Initiating the states.
    public EnemyBasicAttackState BasicAttackState ;
    public EnemySuperAttack_State SuperAttackState ;


    //Variables to cache Instances 
    public EnemyAnimationsLength AnimationsLength;


    //StateMachine Variables (logic and animation)
    //public bool IsCastingAnAbility ; 



    public override void Awake()
    {
        base.Awake();
        CashingEnemyInstances() ;

    }

    private void CashingEnemyInstances()
    {
        BasicAttackState = GetComponent<EnemyBasicAttackState>();
        SuperAttackState = GetComponent<EnemySuperAttack_State>();

        AnimationsLength = GetComponent<EnemyAnimationsLength>();
    }

    public override void Update()
    {
        base.Update();

    }
}
