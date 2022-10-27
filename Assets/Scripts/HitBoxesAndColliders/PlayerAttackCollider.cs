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
        if (other.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy))
            {
                _hitBoxes.Targets.Add(damageableEnemy);
            }
    }


    void OnTriggerExit(Collider other)
    {
        
        if (other.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy) ) 
            {
                // if any problem occurs  
                // && _hitBoxes.Targets.Contains(other.GetComponent<EnemyBase>())
                _hitBoxes.Targets.Remove(damageableEnemy);
            }        
    }

}
