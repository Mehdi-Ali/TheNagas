using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    public CameraTarget _cameraTarget;

    Vector3 controllerPosition;

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
        controllerPosition.x = _cameraTarget.transform.position.x;
        controllerPosition.z = _cameraTarget.transform.position.z + zOffset;
        controllerPosition.y = yOffset;

        this.transform.position = controllerPosition;


    }
}
