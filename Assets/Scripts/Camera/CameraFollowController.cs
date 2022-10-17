using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CameraFollowController : NetworkBehaviour
{
    public CameraTarget _cameraTarget;

    Vector3 controllerPosition;
    Vector3 _cameraPosition;

    [SerializeField]
    float zOffset = -5.0f ;
    [SerializeField]
    float yOffset =  8.0f ;

    // Camera rotation : x = 55 ;  y = z = 0
    // Camera projection : Field of View = 60 ; Axis = Vertical


    public bool ClientConnected = false;

    void Start()
    {
        controllerPosition = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ClientConnected) return ;

        _cameraPosition = _cameraTarget.transform.position ;
        controllerPosition.x = _cameraPosition.x;
        controllerPosition.z = _cameraPosition.z + zOffset;
        controllerPosition.y = yOffset;

        this.transform.position = controllerPosition;


    }
}
