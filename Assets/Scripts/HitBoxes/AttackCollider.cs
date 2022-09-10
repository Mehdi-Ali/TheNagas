using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{

    [SerializeField] PlayerHitBoxes _hitBoxes;
    [SerializeField] public Collider Collider ; 


        private void Awake() 
    {
        _hitBoxes = GetComponentInParent<PlayerHitBoxes>() ;
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
