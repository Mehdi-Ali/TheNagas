using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI ;

public class EnemyStateManger : StateManger
{

    //Initiating the states.
    public EnemyRoamingState RoamingState ;
    public EnemyChasingState ChasingState ;
    public EnemyBasicAttackState BasicAttackState ;
    public EnemySuperAttackState SuperAttackState ;


    //Variables to cache Instances 
    public EnemyAnimationsLength AnimationsLength;
    public EnemyStatics Statics ;
    public NavMeshAgent NavAgent;

    // TODO find object when the client is ON
    public PlayerStateManger Player ;


    //StateMachine Variables (logic and animation)

        public Vector3 x ;
        public Vector3 y ;
        public float z ;


    public override void Awake()
    {
        CashingEnemyInstances() ;
        

        base.Awake();

    }

    private void CashingEnemyInstances()
    {
        RoamingState = GetComponent<EnemyRoamingState>();
        ChasingState = GetComponent<EnemyChasingState>();
        BasicAttackState = GetComponent<EnemyBasicAttackState>();
        SuperAttackState = GetComponent<EnemySuperAttackState>();

        Statics = GetComponent<EnemyStatics>();
        AnimationsLength = GetComponent<EnemyAnimationsLength>();
        NavAgent = GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();

        if (Player == null) {GettingTarget();}
        else if (ReadyToSwitchState)
        {
            x = transform.position ;
            y = Player.gameObject.transform.position ;
            z = Vector3.Distance(transform.position, Player.transform.position);

            //TODO turn the target into an array Targets and make this a for each loop
            if (Vector3.Distance(transform.position, Player.transform.position) < Statics.VisionRange)
            {
                SwitchState(ChasingState);
            }
        }

    }

    public void GettingTarget()
    {
        Player = FindObjectOfType<PlayerStateManger>();
    }
}
