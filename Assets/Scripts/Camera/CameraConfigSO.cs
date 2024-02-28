using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "TurboTowers/Camera/CameraConfig")]
public class CameraConfigSO : ScriptableObject
{
    public Vector3 baseOffsetFromPlayer;
    public Quaternion baseRotation;
    public float fixedHighDifference;
    
    public void SetOffset(ref OffsetData offset)
    {
        Debug.Log("Setting offset to " + offset);
        baseOffsetFromPlayer = offset.posOffset;
        baseRotation = offset.rotation;
        fixedHighDifference = offset.heightDifference;
    }
    
    public OffsetData GetOffset()
    {
        return new OffsetData(baseOffsetFromPlayer, baseRotation, fixedHighDifference);
    }
    
    public Vector3 GetOffsetPosition()
    {
        return baseOffsetFromPlayer;
    }
    
    public Quaternion GetOffsetRotation()
    {
        return baseRotation;
    }
    
    public float GetOffsetHeightDifference()
    {
        return fixedHighDifference;
    }
}
