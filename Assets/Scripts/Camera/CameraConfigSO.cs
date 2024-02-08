using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "TurboTowers/Camera/CameraConfig")]
public class CameraConfigSO : ScriptableObject
{
    public Vector3 baseOffsetFromPlayer;
    public Quaternion baseRotation;
    
    public void SetOffset(Vector3 offset)
    {
        Debug.Log("Setting offset to " + offset);
        baseOffsetFromPlayer = offset;
    }
    
    public void SetRotation(Quaternion rotation)
    {
        Debug.Log("Setting rotation to " + rotation);
        baseRotation = rotation;
    }
    
    
}
