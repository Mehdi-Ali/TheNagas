using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : MonoBehaviour
{
    //change the name from Moving to Cameratarget.

    #region 
    //Camera Stuff
    CameraFollowController _camera;
    void Awake()
    {

        _camera = FindObjectOfType<CameraFollowController>();
        _camera._cameraTarget = this;
        _camera.ClientConnected = true ;
    }

    #endregion

   
}

