using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : NetworkBehaviour
{
    CameraFollowController _camera;
    
    [Client(RequireOwnership = true)]
    public override void OnStartClient()
    {
        base.OnStartClient();

        _camera = FindObjectOfType<CameraFollowController>();
        _camera._cameraTarget = this;
        _camera.ClientConnected = true ;
    }

    [Client(RequireOwnership = true)]
    public override void OnStopClient()
    {
        base.OnStopClient();
       _camera.ClientConnected = false ;
    }   
}

