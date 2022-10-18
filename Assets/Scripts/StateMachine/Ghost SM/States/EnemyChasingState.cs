using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : BaseState
{    
    //A reference for the Player State Manger
    EnemyStateManger _enemy;

    //Variables to store optimized Setter / getter parameter IDs
    int _RunningHash;

    // utilities 
    Vector3 _targetPosition ;

    public void Awake()
    {

        //Caching The Player State Manger
        _enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _RunningHash = Animator.StringToHash("Running");

    }


    public override void EnterState()
    {
        _enemy.Animator.CrossFade(_RunningHash, 0.15f);
        _enemy.NavAgent.speed = _enemy.Statics.ChasingSpeed ;        
    }

    public override void UpdateState()
    {
        _enemy.NavAgent.destination = _enemy.Player.transform.position ;

        if (Vector3.Distance(transform.position, _enemy.Player.transform.position)
                                                     < _enemy.Statics.AttackRange)
        {
            SwitchState(_enemy.BasicAttackState);
        }
        else if (Vector3.Distance(transform.position, _enemy.Player.transform.position)
                                                       > _enemy.Statics.DropAggroRange)
        {
            SwitchState(_enemy.IdleState);
        }

    }

    public override void ExitState()
    {

    }

    private void SwitchState(BaseState state)
    {
        _enemy.SwitchState(state) ;
        _enemy.NavAgent.destination = transform.position ;
    }


}