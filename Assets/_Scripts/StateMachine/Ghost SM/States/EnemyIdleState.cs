using UnityEngine;

public class EnemyIdleState : BaseState
{
    EnemyStateManger _Enemy;

    int _IdleHash;

    public  void Awake()
    {
        _Enemy = GetComponent<EnemyStateManger>();
        _IdleHash = Animator.StringToHash("Idle");
    }

    public override void EnterState()
    {
        _Enemy.NetworkAnimator.CrossFade(_IdleHash, 0.15f, 0);
        var statics = _Enemy.Statics;
        
    }

    public override void UpdateState(){}

    public override void ExitState(){}

    private void GoRoam()
    {
        _Enemy.SwitchState(_Enemy.RoamingState);
    }

}
