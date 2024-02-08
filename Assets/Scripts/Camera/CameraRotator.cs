using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraRotator : MonoBehaviour
{
    // Config:
    [SerializeField] float smoothSpeed = 0.125f; // Adjust for smoother movement
    [SerializeField] private CameraConfigSO cameraConfig;
    
    // Caching:
    private Transform turret; // The stationary turret
    private Vector3 offset; // Offset from the turret
    private float fixedHeightDifference; // Fixed height difference from the turret

    private void Awake()
    {
        turret = GameObject.FindWithTag("Player").transform.Find("Head").transform.Find("Head_Rot");
    }

    private void Start()
    {
        offset = transform.localPosition - turret.localPosition;
        fixedHeightDifference = transform.position.y - turret.position.y;
    }

    void LateUpdate()
    {
        // Calculate the desired position with offset
        Vector3 desiredPosition = turret.position + turret.TransformDirection(offset);
        desiredPosition.y = turret.position.y + fixedHeightDifference;

        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera position
        transform.position = smoothedPosition;

        // Make sure the camera always faces the same direction as the turret
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, turret.eulerAngles.y, turret.eulerAngles.z), smoothSpeed);
    }

#if UNITY_EDITOR
    public void SetOffset()
    {
        cameraConfig.SetOffset(offset);
    }
    
    public void SetRotation()
    {
        cameraConfig.SetRotation(transform.rotation);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraRotator))]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        CameraRotator cameraRotator = (CameraRotator)target;

        if (GUILayout.Button("Set New Offset"))
        {
            cameraRotator.SetOffset();
        }
    
        if (GUILayout.Button("Set New Rotation"))
        {
            cameraRotator.SetOffset();
        }
    }
}
#endif

