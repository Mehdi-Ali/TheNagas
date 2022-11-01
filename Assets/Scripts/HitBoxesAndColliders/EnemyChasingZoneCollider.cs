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
                _hitBoxes.TargetsToChase.Add(targetToChase);

                if (_hitBoxes.TargetsToChase.Count == 1)
                    _enemy.TargetPlayer = targetToChase;
                else
                    UpdatingTheTarget();

                _enemy.StartChasing();
            }
    }


    void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent<PlayerBase>(out PlayerBase targetToChase) ) 
            {
                _hitBoxes.TargetsToChase.Remove(targetToChase);

                if (_hitBoxes.TargetsToChase.Count == 0)
                {
                    _enemy.TargetPlayer = null;
                    return;
                }
                
                UpdatingTheTarget();
                
                _enemy.StartChasing();
            }        
    }

    private void UpdatingTheTarget()
    {
        var smallestDistance = _collider.radius;

        foreach(PlayerBase targetToChase in _hitBoxes.TargetsToChase)
        {
            var  _distance = Vector3.Distance(this.transform.position, targetToChase.transform.position);
            if (_distance > smallestDistance ) continue ;

            smallestDistance = _distance;
            _enemy.TargetPlayer = targetToChase;
        }
    }


}
