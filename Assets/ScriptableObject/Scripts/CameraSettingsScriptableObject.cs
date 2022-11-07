using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "ScriptableObject/Camera/CameraSettings")]
public class CameraSettingsScriptableObject : ScriptableObject
{
    public Vector3 Rotation;
    public Vector3 Offset;
    
    // Maybe add the field of view.
}
