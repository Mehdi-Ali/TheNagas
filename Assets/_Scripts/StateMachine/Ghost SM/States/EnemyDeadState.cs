
using FishNet.Object;
using UnityEngine;

public class EnemyDeadState : BaseState 
{
    EnemyStateManger _enemy;

    protected int _DeadHash;
    
    public void Awake()
    {
        _enemy = GetComponent<EnemyStateManger>();

        _DeadHash = Animator.StringToHash("Dead");

    }
    public override void EnterState()
    {
        RpcDisableCollider();
        _enemy.RpcHitBoxDisplay(false);
        _enemy.NetworkAnimator.CrossFade(_DeadHash, 0.15f, 0);
        _enemy.ReadyToSwitchState = false;
    }

    [ObserversRpc]
    private void RpcDisableCollider()
    {
        GetComponent<CapsuleCollider>().enabled = false;
    }

    public override void UpdateState(){}

    public override void ExitState(){}

}
