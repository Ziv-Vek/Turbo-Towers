using System.Collections;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private Transform turretHead;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float pitchSpeed = 15f;
    [SerializeField, Range(0, 90)] private float maxPitchAngle = 60f;
    [SerializeField, Range(-45, 0)] private float minPitchAngle = 0;
    [SerializeField] private HoldClickableButton powerBtn;

    private int inputStyle = 1;

    private const float FixedTurretRotation = -25;
    private bool isHorizontalRotationActive = true;

    private void OnEnable()
    {
        PlayerController.onHorizontalTouchDrag += HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag += VerticalTouchDragHandler;

        GetComponent<PlayerAttacker>().OnTurretPowering += ToggleHorizontalRotation;
        GetComponent<PlayerAttacker>().OnTurretFired += ToggleHorizontalRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretPowering += ToggleHorizontalRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretFired += ToggleHorizontalRotation;

        StartCoroutine(HandleInputListeners());
    }

    private void OnDisable()
    {
        PlayerController.onHorizontalTouchDrag -= HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag -= VerticalTouchDragHandler;
        
        GetComponent<PlayerAttacker>().OnTurretPowering -= ToggleHorizontalRotation;
        GetComponent<PlayerAttacker>().OnTurretFired -= ToggleHorizontalRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretPowering -= ToggleHorizontalRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretFired -= ToggleHorizontalRotation;
        
        InputSwitchHandler.Instance.OnInputStyleSelect -= InputStyleSelectHandler;
    }

    private IEnumerator HandleInputListeners()
    {
        if (!InputSwitchHandler.Instance) yield return null;
        
        InputSwitchHandler.Instance.OnInputStyleSelect += InputStyleSelectHandler;
    }

    private void InputStyleSelectHandler(int inputStyle)
    {
        this.inputStyle = inputStyle;

        if (this.inputStyle == 2)
        {
            SetTurretToFixedRotation();
        }
    }

    private void HorizontalTouchDragHandler(int touchDragDirection)
    {
        if (!isHorizontalRotationActive) return;
        
        turretHead.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime * touchDragDirection));
    }
    
    private void VerticalTouchDragHandler(int touchDragDirection)
    {
        if (inputStyle != 1) return;
        
        float angleX = 360 - turretPivot.eulerAngles.x;

        if (touchDragDirection > 0)
        {
            // Raise turret's pivot:
            if (angleX <= maxPitchAngle || angleX >= 360 - maxPitchAngle)
            {
                turretPivot.Rotate(Vector3.right * -pitchSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Lower turret's pivot:
            if (turretPivot.eulerAngles.x > 180 || turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle))
            {
                turretPivot.Rotate(Vector3.right * pitchSpeed * Time.deltaTime);
            }
        }
    }

    private void SetTurretToFixedRotation()
    {
        // turretPivot.Rotate(-25, 0, 0);
        turretPivot.RotateAround(turretPivot.localPosition, Vector3.right, -25);
        // var rot = Quaternion.Euler(FixedTurretRotation, 0, 0);
        //
        // turretPivot.SetPositionAndRotation(turretPivot.transform.localPosition, rot);
    }

    private void ToggleHorizontalRotation()
    {
        isHorizontalRotationActive = !isHorizontalRotationActive;
    }
    
}
