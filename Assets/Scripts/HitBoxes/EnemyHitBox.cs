using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] PlayerHitBoxesAndColliders _hitBoxes;

    private void Awake() 
    {
        _hitBoxes = GetComponentInParent<PlayerHitBoxesAndColliders>() ;
    }
}
