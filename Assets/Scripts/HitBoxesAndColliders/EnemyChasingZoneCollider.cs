using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingZoneCollider : MonoBehaviour
{
    EnemyStateManger _enemy;

    [SerializeField] EnemyHitBoxesAndColliders _hitBoxes;

    [SerializeField] public SphereCollider _collider;

    void Awake()
    {
        _enemy = GetComponentInParent<EnemyStateManger>();
        _hitBoxes = GetComponentInParent<EnemyHitBoxesAndColliders>();

        SetChasingRange();
    }

    private void SetChasingRange()
    {
        var visionRange = _enemy.Statics.VisionRange;

        _collider.radius = visionRange;
        _collider.center = new Vector3(0f, 0f, visionRange / 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerBase>(out PlayerBase targetToChase))
            {
                if (!targetToChase.IsAlive) return;
                _hitBoxes.TargetsToChase.Add(targetToChase);
                
                if (_enemy.CurrentState != _enemy.ChasingState)
                    _enemy.SwitchState(_enemy.ChasingState);
            }
    }


    void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent<PlayerBase>(out PlayerBase targetToChase) ) 
            {
                _hitBoxes.TargetsToChase.Remove(targetToChase);
            }        
    }



}
