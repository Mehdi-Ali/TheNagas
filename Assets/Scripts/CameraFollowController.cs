using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CameraFollowController : NetworkBehaviour
{
    public Moving _moving;

    Vector3 controllerPostion;

    [SerializeField]
    float zOffset = -2.6f ;
    [SerializeField]
    float yOffset =  4f ;

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
        controllerPostion.x = _moving.transform.position.x;
        controllerPostion.z = _moving.transform.position.z + zOffset;
        controllerPostion.y = yOffset;

        this.transform.position = controllerPostion;


    }
}
