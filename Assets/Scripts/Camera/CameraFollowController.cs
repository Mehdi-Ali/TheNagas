using UnityEngine;

public class CameraFollowController : MonoBehaviour
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

    public void LateUpdate()
    {
        if (!ClientConnected) return ;

        _cameraPosition = _cameraTarget.transform.position ;
        // TODO once this parameters are set and done we get them only once in the start method.
        Vector3 offset = _cameraSettings.Offset ;
        Vector3 rotation = _cameraSettings.Rotation ;

        controllerPosition = _cameraPosition + new Vector3(0f , offset.y, offset.z);
        
        this.transform.position = controllerPosition;
        this.transform.eulerAngles = rotation;

    }
}
