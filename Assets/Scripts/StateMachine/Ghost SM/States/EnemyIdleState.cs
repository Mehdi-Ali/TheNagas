using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IdleState
{
    //A reference for the Player State Manger
    EnemyStateManger _Enemy;


    public override void Awake()
    {
        base.Awake();

        //Caching The Player State Manger
        _Enemy = GetComponent<EnemyStateManger>();
        
    }
    public override void EnterState()
    {
        _Enemy.Animator.CrossFade(_Idle, 0.15f);

        Invoke(nameof(GoRoam), Random.Range(_Enemy.Statics.MinRoamingPause , _Enemy.Statics.MaxRoamingPause));

    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

    private void GoRoam()
    {
        _Enemy.SwitchState(_Enemy.RoamingState);
    }

}
