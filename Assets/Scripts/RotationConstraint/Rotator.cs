using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
Vector3 _newRotation;
[SerializeField] float _rotationSpeed = 150f ;

private void Start() 
{
    _newRotation.x += 52f ;    
    _newRotation.z += 42f ;    
}


    void Update()
    {
        _newRotation.x += _rotationSpeed * Time.deltaTime ;  
        _newRotation.y += _rotationSpeed * Time.deltaTime ;  

        transform.eulerAngles = _newRotation ;
    }
}
