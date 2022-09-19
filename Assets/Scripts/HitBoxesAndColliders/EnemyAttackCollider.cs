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
        if (other.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy))
            {
                _hitBoxes.Targets.Add(damageableEnemy);
            }
    }


    void OnTriggerExit(Collider other)
    {
        // if any problem occurs  
        // && _hitBoxes.Targets.Contains(other.GetComponent<EnemyBase>())
        
        if (other.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy) ) 
            {
                _hitBoxes.Targets.Remove(damageableEnemy);
            }        
    }

}
