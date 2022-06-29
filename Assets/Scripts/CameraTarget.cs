using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTarget : NetworkBehaviour
{
    //change the name from Moving to Cameratarget.

    #region 
    //Camera Stuff
    CameraFollowController _camera;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) return;

        _camera = FindObjectOfType<CameraFollowController>();
        _camera._cameraTarget = this;
        _camera.ClinetConnected = true ;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (base.IsOwner) _camera.ClinetConnected = false ;
    }
    //End Camera stuff
    #endregion

       

   

    // the equivalante of "if (!base.IsOwner) return;"
    //is : 
    //[Client(RequireOwnership = true)]


   
}

