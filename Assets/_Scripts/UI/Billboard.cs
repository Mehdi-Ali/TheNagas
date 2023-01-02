using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform _cameraTransform ;

    void LateUpdate()
    {
        if (_cameraTransform == null)
        _cameraTransform = Camera.main.transform ;
        if (_cameraTransform == null)
        return;

        transform.LookAt(transform.position + _cameraTransform.forward);
    }
}
