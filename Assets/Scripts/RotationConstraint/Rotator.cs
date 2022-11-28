using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Vector3 _newMovement;
    // [SerializeField] Vector2 _movementSpeed = new(1f, 1f);
    Vector3 _newRotation;
    [SerializeField] Vector2 _rotationSpeed = new(150f, 150f);


    void Update()
    {
        // _newMovement.x += _movementSpeed.x * Time.deltaTime * Random.Range(-1f, 1f) ;  
        // _newMovement.y += _movementSpeed.y * Time.deltaTime * Random.Range(-1f, 1f);  

        // transform.position = _newMovement ;

        _newRotation.x += _rotationSpeed.x * Time.deltaTime ;  
        _newRotation.y += _rotationSpeed.y * Time.deltaTime ;  

        transform.eulerAngles = _newRotation ;
    }
}
