using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI ;

public class EnemyStateManger : StateManger
{

    //Initiating the states.
    public EnemyRoamingState RoamingState ;
    public EnemyBasicAttackState BasicAttackState ;
    public EnemySuperAttackState SuperAttackState ;


    //Variables to cache Instances 
    public EnemyAnimationsLength AnimationsLength;
    public EnemyStatics Statics ;
    public NavMeshAgent NavAgent;


    //StateMachine Variables (logic and animation)



    public override void Awake()
    {
        CashingEnemyInstances() ;

        base.Awake();

    }

    private void CashingEnemyInstances()
    {
        RoamingState = GetComponent<EnemyRoamingState>();
        BasicAttackState = GetComponent<EnemyBasicAttackState>();
        SuperAttackState = GetComponent<EnemySuperAttackState>();

        Statics = GetComponent<EnemyStatics>();
        AnimationsLength = GetComponent<EnemyAnimationsLength>();
        NavAgent = GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();

    }
}
