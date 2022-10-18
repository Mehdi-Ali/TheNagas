using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRoamingState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    //Variables to store optimized Setter / getter parameter IDs
    int _RunningHash;

    // utilities 
    private Vector3 _roamingPos ;

    public void Awake()
    {

        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _RunningHash = Animator.StringToHash("Running");

        _roamingPos = transform.position ;
    }


    public override void EnterState()
    {
        _enemy.NetworkAnimator.CrossFade(_RunningHash, 0.15f, 0);
        _enemy.NavAgent.speed = _enemy.Statics.RoamingSpeed ;

        _roamingPos = GetRandomPosition();
        _roamingPos.y = transform.position.y ;


        
    }

    public override void UpdateState()
    {
        _enemy.NavAgent.destination = _roamingPos ;

        if (Vector3.Distance(transform.position, _roamingPos) < 1.0f)
        {
            StopRoaming();
        }
    }

    public override void ExitState()
    {

    }

    private void StopRoaming()
    {
        _enemy.SwitchState(_enemy.IdleState);
        _enemy.NavAgent.destination = transform.position ;
    }

    private Vector3 GetRandomPosition()
    {
        //     Starting Position  +                    Offset 
        return transform.position + GetRandomDirection() * Random.Range( 2.5f , 5.0f );
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f,1f), 0 , Random.Range(-1f,1f)).normalized;
    }

}
