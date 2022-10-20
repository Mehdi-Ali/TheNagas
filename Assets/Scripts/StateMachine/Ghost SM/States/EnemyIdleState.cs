using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : BaseState
{
    //A reference for the Player State Manger
    EnemyStateManger _Enemy;

    //Variables to store optimized Setter / getter parameter IDs
    protected int _Idle;

    public  void Awake()
    {
        //Caching The Player State Manger
        _Enemy = GetComponent<EnemyStateManger>();
        
        //caching Hashes
        _Idle = Animator.StringToHash("Idle");
        
    }
    public override void EnterState()
    {
        _Enemy.NetworkAnimator.CrossFade(_Idle, 0.15f, 0);

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
