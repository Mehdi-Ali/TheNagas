using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{

    [SerializeField] PlayerHitBoxesAndColliders _hitBoxes;
    [SerializeField] public Collider Collider ; 


    private void Awake() 
    {
        _hitBoxes = GetComponentInParent<PlayerHitBoxesAndColliders>() ;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyBase>(out EnemyBase target))
            {
                _hitBoxes.Targets.Add(target);
            }
    }


    void OnTriggerExit(Collider other)
    {
        
        if (other.TryGetComponent<EnemyBase>(out EnemyBase target) ) 
            {
                _hitBoxes.Targets.Remove(target);
            }        
    }

}
