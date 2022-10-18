
using UnityEngine;

public class EnemyDeadState : DeadState 
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
        GetComponent<CapsuleCollider>().enabled = false ;
        
        _Enemy.NetworkAnimator.CrossFade(_DeadHash, 0.15f, 0);
        _Enemy.ReadyToSwitchState = false;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

}
