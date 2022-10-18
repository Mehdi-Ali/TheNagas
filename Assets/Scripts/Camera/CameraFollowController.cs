using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CameraFollowController : NetworkBehaviour
{
    [Header("Scriptable Objects")]

    [SerializeField]
    private CameraSettingsScriptableObject _cameraSettings;
    // Camera projection : Field of View = 60 ; Axis = Vertical
    
    [Space(10)]

    public CameraTarget _cameraTarget;

    Vector3 controllerPosition;
    Vector3 _cameraPosition;



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
        // TODO oce this parameters are set and done we get them only once in the start method.
        Vector3 offset = _cameraSettings.Offset ;
        Vector3 rotation = _cameraSettings.Rotation ;

        controllerPosition.x = _cameraPosition.x;
        controllerPosition.z = _cameraPosition.z + offset.z;
        controllerPosition.y = offset.y;

        this.transform.position = controllerPosition;
        this.transform.eulerAngles = rotation;

    }
}
