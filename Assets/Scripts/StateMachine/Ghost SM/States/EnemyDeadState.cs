
using UnityEngine;

public class EnemyDeadState : BaseState 
{
    //A reference for the Player State Manger
    EnemyStateManger _Enemy;

    //Variables to store optimized Setter / getter parameter IDs
    protected int _DeadHash;
    
    public void Awake()
    {
        //Caching The Player State Manger
        _Enemy = GetComponent<EnemyStateManger>();

        //caching Hashes
        _DeadHash = Animator.StringToHash("Dead");

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
