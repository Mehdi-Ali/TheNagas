using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    PlayerHitBoxesAndColliders _hitBoxes;
    public bool Movable;

    private void Awake() 
    {
        _hitBoxes = GetComponentInParent<PlayerHitBoxesAndColliders>() ;
    }

    private void OnEnable() 
    {
        _hitBoxes.RotationConstraint.constraintActive = Movable ;
        
        if (!Movable)
        { 
            _hitBoxes.RotationConstraint.transform.localRotation  = Quaternion.Euler(Vector3.zero);
            _hitBoxes.RotationConstraint.transform.localPosition = Vector3.zero;
            _hitBoxes.transform.localRotation  = Quaternion.Euler(Vector3.zero);
            _hitBoxes.transform.localPosition = Vector3.zero ;
        }
    }

}
