using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CameraFollowController : NetworkBehaviour
{
    public CameraTarget _cameraTarget;

    Vector3 controllerPostion;

    [SerializeField]
    float zOffset = -5.0f ;
    [SerializeField]
    float yOffset =  8.0f ;

    // Camera rotation : x = 55 ;  y = z = 0
    // Camera projection : Field of View = 60 ; Axis = Vertical


    public bool ClinetConnected = false;

    void Start()
    {
        controllerPostion = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ClinetConnected) return ;
        controllerPostion.x = _cameraTarget.transform.position.x;
        controllerPostion.z = _cameraTarget.transform.position.z + zOffset;
        controllerPostion.y = yOffset;

        this.transform.position = controllerPostion;


    }
}
