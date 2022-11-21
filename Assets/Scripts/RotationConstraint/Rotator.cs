using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
Vector3 _newRotation;
[SerializeField] Vector2 _rotationSpeed = new(150f, 150f);


    void Update()
    {
        _newRotation.x += _rotationSpeed.x * Time.deltaTime ;  
        _newRotation.y += _rotationSpeed.y * Time.deltaTime ;  

        transform.eulerAngles = _newRotation ;
    }
}
