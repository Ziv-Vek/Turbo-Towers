using System.Collections;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] public Transform turretHead;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float pitchSpeed = 15f;
    [SerializeField, Range(0, 90)] private float maxPitchAngle = 60f;
    [SerializeField, Range(-45, 0)] private float minPitchAngle = 0;
    [SerializeField] private HoldClickableButton powerBtn;

    private int inputStyle = 1;

    private const float FixedTurretRotation = -25;
    private bool isHorizontalRotationActive = true;

    [Header("Smooth Rotation")]
    public bool isSmoothImplemented = true;
    private float currentYAngle = 0f;
    private float yVelocity = 0f;
    [SerializeField] float smoothTime = 0.3f;

    [Header("Lerp Rotation")]
    public bool isLerpImplemented = false;
    [SerializeField] private float rotationLerpTime = 0.1f; // Time taken to reach the target rotation
    private Quaternion targetRotation;

    private void Start()
    {
        // Initialize target rotation to the current rotation of the turret head
        targetRotation = turretHead.rotation;
    }

    private void OnEnable()
    {
        PlayerController.onHorizontalTouchDrag += HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag += VerticalTouchDragHandler;

        GetComponent<PlayerAttacker>().OnTurretPowering += DisableRotation;
        GetComponent<PlayerAttacker>().OnTurretFired += EnableRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretPowering += DisableRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretFired += EnableRotation;

        StartCoroutine(HandleInputListeners());
    }

    private void OnDisable()
    {
        PlayerController.onHorizontalTouchDrag -= HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag -= VerticalTouchDragHandler;

        GetComponent<PlayerAttacker>().OnTurretPowering -= DisableRotation;
        GetComponent<PlayerAttacker>().OnTurretFired -= EnableRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretPowering -= DisableRotation;
        GetComponent<PlayerSlideAttacker>().OnTurretFired -= EnableRotation;

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

        RotateTurretHead(touchDragDirection);
    }

    public void RotateTurretHead(float angle)
    {
        if (!isHorizontalRotationActive) return;

        // implements smooth rotation, or lerp rotation, or normal rotation
        //TODO: delete two other implementations not used
        if (isSmoothImplemented)
        {
            float targetAngle = currentYAngle + angle * rotationSpeed;
            currentYAngle = Mathf.SmoothDampAngle(currentYAngle, targetAngle, ref yVelocity, smoothTime);

            turretHead.rotation = Quaternion.Euler(0, currentYAngle, 0);
        }
        else if(isLerpImplemented)
        {
            /*targetRotation = Quaternion.Euler(0, angle, 0);
            turretHead.rotation = Quaternion.Lerp(turretHead.rotation, targetRotation, rotationLerpTime);*/


            // Calculate target rotation based on the angle
            float yRotation = turretHead.eulerAngles.y + angle * rotationSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(0, yRotation, 0);

            // Smoothly interpolate to the target rotation
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, rotationLerpTime);
        }
        else
        {
            turretHead.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime * angle));
        }
        
    }

    private void VerticalTouchDragHandler(int touchDragDirection)
    {
        if (inputStyle != 1) return;

        if (touchDragDirection > 0)
        {
            RaiseTurretPivot();
        }
        else
        {
            LowerTurretPivot();
        }
    }

    public void RaiseTurretPivot()
    {
        float angleX = 360 - turretPivot.eulerAngles.x;

        if (angleX <= maxPitchAngle || angleX >= 360 - maxPitchAngle)
        {
            turretPivot.Rotate(Vector3.right * (-pitchSpeed * Time.deltaTime));
        }
    }

    public void LowerTurretPivot()
    {
        float angleX = 360 - turretPivot.eulerAngles.x;

        if (angleX > 180 || angleX <= Mathf.Abs(minPitchAngle))
        {
            turretPivot.Rotate(Vector3.right * (pitchSpeed * Time.deltaTime));
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


    private void DisableRotation()
    {
        isHorizontalRotationActive = false;
    }

    private void EnableRotation()
    {
        isHorizontalRotationActive = true;
    }
}