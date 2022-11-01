using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{

    [SerializeField] EnemyHitBoxesAndColliders _hitBoxes;
    [SerializeField] public Collider Collider ; 


    private void Awake() 
    {
        _hitBoxes = GetComponentInParent<EnemyHitBoxesAndColliders>() ;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerBase>(out PlayerBase target))
            {
                _hitBoxes.Targets.Add(target);
            }
    }


    void OnTriggerExit(Collider other)
    {
        
        if (other.TryGetComponent<PlayerBase>(out PlayerBase target) ) 
            {
                _hitBoxes.Targets.Remove(target);
            }        
    }

}
