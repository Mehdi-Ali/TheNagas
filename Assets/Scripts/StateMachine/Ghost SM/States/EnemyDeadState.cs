
using UnityEngine;

public class EnemyDeadState : BaseState 
{
    EnemyStateManger _Enemy;

    protected int _DeadHash;
    
    public void Awake()
    {
        _Enemy = GetComponent<EnemyStateManger>();

        _DeadHash = Animator.StringToHash("Dead");

    }
    public override void EnterState()
    {
        GetComponent<CapsuleCollider>().enabled = false ;
        _Enemy.NetworkAnimator.CrossFade(_DeadHash, 0.15f, 0);
        _Enemy.ReadyToSwitchState = false;
    }

    public override void UpdateState(){}

    public override void ExitState(){}

}
