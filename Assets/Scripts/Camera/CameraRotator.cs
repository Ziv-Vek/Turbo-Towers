using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] Transform turret; // The stationary turret
    [SerializeField] float smoothSpeed = 0.125f; // Adjust for smoother movement
    
    private Vector3 offset; // Offset from the turret
    private float fixedHeightDifference; // Fixed height difference from the turret


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
}
