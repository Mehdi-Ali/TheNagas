using FishNet;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Scriptable Objects")]

    [SerializeField]
    private CameraSettingsScriptableObject _cameraSettings;
    // Camera projection : Field of View = 60 ; Axis = Vertical
    
    [Space(10)]
    public PlayerBase CameraTarget;

    private Vector3 _controllerPosition;
    private Vector3 _cameraPosition;

    void Awake()
    {
        var characters = FindObjectsOfType<Character>();
        foreach (var character in characters)
        {
            character.SettingCamera();
        }
    }

    public void LateUpdate()
    {
        if (!InstanceFinder.IsClient || CameraTarget == null)
            return;

        // ! once this parameters are set and done we get them only once in the start method.
        Vector3 offset = _cameraSettings.Offset ;
        Vector3 rotation = _cameraSettings.Rotation ;

        _cameraPosition = CameraTarget.transform.position ;
        _controllerPosition = _cameraPosition + new Vector3(0f , offset.y, offset.z);
        
        this.transform.position = _controllerPosition;
        this.transform.eulerAngles = rotation;

    }
}
