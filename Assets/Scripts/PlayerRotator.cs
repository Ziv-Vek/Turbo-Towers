using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private Transform turretHead;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float pitchSpeed = 15f;
    [SerializeField, Range(0, 90)] private float maxPitchAngle = 60f;
    [SerializeField, Range(-45, 0)] private float minPitchAngle = 0;

    private void OnEnable()
    {
        PlayerController.onTouchDragRight += TouchDragRightHandler;
        PlayerController.onTouchDragLeft += TouchDragLeftHandler;
        PlayerController.onTouchDragUp += TouchDragUpHandler;
        PlayerController.onTouchDragDown += TouchDragDownHandler;
        
    }

    private void OnDisable()
    {
        PlayerController.onTouchDragRight -= TouchDragRightHandler;
        PlayerController.onTouchDragLeft -= TouchDragLeftHandler;
        PlayerController.onTouchDragUp -= TouchDragUpHandler;
        PlayerController.onTouchDragDown -= TouchDragDownHandler;
    }

    private void TouchDragRightHandler()
    {
        turretHead.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void TouchDragLeftHandler()
    {
        turretHead.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
    
    private void TouchDragUpHandler()
    {
        // Raise turret's pivot:
        float angleX = 360 - turretPivot.eulerAngles.x;
        
        if (angleX <= maxPitchAngle || angleX >= 360 - maxPitchAngle)
        {
            turretPivot.Rotate(Vector3.right * -pitchSpeed * Time.deltaTime);
        }
    }

    private void TouchDragDownHandler()
    {
        Debug.Log(turretPivot.eulerAngles.x.ToString());
        // Lower turret's pivot:
        if (turretPivot.eulerAngles.x > 180 || turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle))
        {
            turretPivot.Rotate(Vector3.right * pitchSpeed * Time.deltaTime);
        }
    }
}
