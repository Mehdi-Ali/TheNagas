using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI ;

public class EnemyStateManger : StateManger
{
    [SerializeField] // TODO how to put it at the top in the editor 
    public EnemyStaticsScriptableObject Statics ;

    //Initiating the states.
    public EnemyRoamingState RoamingState ;
    public EnemyChasingState ChasingState ;
    public EnemyBasicAttackState BasicAttackState ;
    public EnemySuperAttackState SuperAttackState ;


    //Variables to cache Instances 
    public EnemyAnimationsLength AnimationsLength;
    public NavMeshAgent NavAgent;
    public EnemyHitBoxesAndColliders HitBoxes ;
    public EnemyHitBox ActiveHitBox;
    public PlayerAttackCollider ActiveAttackCollider;
    //get animator component here and get it in other instance from here


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

        AnimationsLength = GetComponent<EnemyAnimationsLength>();
        NavAgent = GetComponent<NavMeshAgent>();
        HitBoxes = GetComponentInChildren<EnemyHitBoxesAndColliders>();
    }

    public override void Update()
    {
        base.Update();

        //TODO turn the target into an array Targets and make this a for each loop
        if (Player == null) {GettingTarget();}
        else if (ReadyToSwitchState &
                 Vector3.Distance(transform.position, Player.transform.position) < Statics.VisionRange)
        {
            SwitchState(ChasingState);
        }

    }

    public void GettingTarget()
    {
        Player = FindObjectOfType<PlayerStateManger>();
    }
}
