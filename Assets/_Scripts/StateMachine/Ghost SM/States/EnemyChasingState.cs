using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : BaseState
{    
    private EnemyStateManger _enemy;
    private EnemyStaticsScriptableObject _statics;

    int _RunningHash;

    public void Awake()
    {
        _enemy = GetComponent<EnemyStateManger>();
        _statics = _enemy.Statics;
        _RunningHash = Animator.StringToHash("Running");
    }

    public override void EnterState()
    {
        _enemy.NetworkAnimator.CrossFade(_RunningHash, 0.15f, 0);
        _enemy.NavAgent.speed = _statics.ChasingSpeed;
    }

    public override void UpdateState(){}

    public override void OnTickState()
    {
        base.OnTickState();

        if (!GettingTarget()) return;

        _enemy.NavAgent.destination = _enemy.TargetPlayer.transform.position ;

        float distance = Vector3.Distance(transform.position, _enemy.TargetPlayer.transform.position) ;

        if (distance < _statics.AttackRange)
            SwitchState(_enemy.BasicAttackState);
    }

    private bool GettingTarget()
    {
        float smallestDistance;

        if (_enemy.TargetPlayer != null)
            smallestDistance = Vector3.Distance(transform.position, _enemy.TargetPlayer.transform.position);
        else
            smallestDistance = _statics.VisionRange * 1.5f ;

        foreach(PlayerBase targetToChase in _enemy.HitBoxes.TargetsToChase)
        {
            if (!targetToChase.IsAlive) continue;
            var  distance = Vector3.Distance(transform.position, targetToChase.transform.position);
            if (distance > smallestDistance ) continue ;

            smallestDistance = distance;
            _enemy.TargetPlayer = targetToChase;
        }

        if (smallestDistance >= _statics.VisionRange * 1.5f)
        {
            _enemy.TargetPlayer = null;
            SwitchState(_enemy.IdleState);
            return false;
        }

        else
            return true;
    }

    public override void ExitState(){}

    private void SwitchState(BaseState state)
    {
        _enemy.SwitchState(state) ;
        _enemy.NavAgent.destination = transform.position ;
    }


}