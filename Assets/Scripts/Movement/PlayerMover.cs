using System;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Movement;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMover : MonoBehaviour
{
    // Stats Config:
    [SerializeField] public float maxMovementSpeed = 2.0f;
    [SerializeField] private float maxPitchSpeed = 15f;
    [SerializeField] float turnSmoothTime = 0.1f;

    // Variables:
    float turnSmoothVelocity;
    Vector3 playerVelocity;
    Vector2 startTouchPos;
    bool isMovementAllowed = true;

    // Cached ref:
    public FloatingJoystick joystick = null;

    // from PlayerRotator
    [Space] [SerializeField] public Transform turretHead;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform turretPivot;
    [SerializeField, Range(-90, 90)] private float maxPitchAngle = 60f;
    [SerializeField, Range(-45, 45)] private float minPitchAngle = 0;
    [SerializeField] private HoldClickableButton powerBtn;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private bool isReversedPitch;
    
    
    private bool isHorizontalRotationActive = true;

    [Header("Smooth Rotation")] [Tooltip("If true, smooth damp rotation. If false, smooth lerp rotation")] public bool isSmoothImplemented = true;
    private float currentYAngle = 0f;
    private float yVelocity = 0f;
    [SerializeField] float smoothTime = 0.3f;

    
    [SerializeField] private float rotationLerpTime = 0.1f; // Time taken to reach the target rotation
    private Quaternion targetRotation;
    private Quaternion cannonPivotRotation;
    private float currentXAngle = 0f;
    private float xVelocity = 0f;

    private void OnEnable()
    {
        TouchManager.onTouchStarted += StartMove;
        TouchManager.onTouchEnded += CancelMovement;
        TouchManager.onTouchPerformed += Move;
        
        
        /*GetComponent<PlayerAttacker>().OnTurretPowering += DisableRotation;
        GetComponent<PlayerAttacker>().OnTurretFired += EnableRotation;*/
    }

    private void OnDisable()
    {
        TouchManager.onTouchStarted -= StartMove;
        TouchManager.onTouchEnded -= CancelMovement;
        TouchManager.onTouchPerformed -= Move;
        
        /*GetComponent<PlayerAttacker>().OnTurretPowering -= DisableRotation;
        GetComponent<PlayerAttacker>().OnTurretFired -= EnableRotation;*/
    }

    private void Start()
    {
        joystick = FloatingJoystick.Instance;
        if (!joystick) throw new Exception("FloatingJoystick is missing.");
    }
    private void Update()
    {
        Vector3 currentPosition = transform.position;
        transform.position = currentPosition;
    }
    
    public void StartMove(Vector2 screenTouchPos)
    {
        startTouchPos = screenTouchPos;
    }

    public void Move(Vector2 screenTouchPos)
    {
        if (!isHorizontalRotationActive) return;
        if (!isMovementAllowed)
        {
            return;
        }

        //float movementSpeed = GetMovementSpeed(screenTouchPos);
        //(float movementSpeed, float pitchSpeed) = GetMovementSpeed(screenTouchPos);
        var movementSpeed = GetMovementSpeed(screenTouchPos);
        
        Vector2 screenMovementVector = screenTouchPos - startTouchPos;
        Vector2 direction = screenMovementVector.normalized;
        //Vector3 direction = new Vector3(screenMovementVector.x, 0, screenMovementVector.y).normalized;
        
        //RotateTurretHead(direction.x);
        //controller.Move(direction * (movementSpeed * Time.deltaTime));
        
        

        if (isSmoothImplemented)
        {
            SmoothDampRotate(direction.x, movementSpeed.rotationSpeed);
            SmoothDampPitch(direction.y, movementSpeed.pitchSpeed);
            //ClampPitch(direction.y);
        }
        else
        {
            SmoothLerpRotation(direction.x, movementSpeed.rotationSpeed);
            SmoothLerpPitch(direction.y, movementSpeed.pitchSpeed);
            ClampPitch(direction.y);
        }

        //float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //float angle = Mathf.SmoothDampAngle(turretHead.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
        //turretHead.rotation = Quaternion.Euler(0, angle, 0);

        //UpdateAnimator(direction * movementSpeed);
    }

    private void ClampPitch(float directionY)
    {
        float fixedDirection = !isReversedPitch ? -directionY : directionY;

        if (fixedDirection > 0)
        {
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x < 360 - maxPitchAngle && turrenEulerAngles.x > 180)
                ? 360 - maxPitchAngle
                : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        } else if (fixedDirection < 0)
        {
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x > Mathf.Abs(minPitchAngle) && turrenEulerAngles.x < 180)
                ? Mathf.Abs(minPitchAngle)
                : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        }
        
        
        //Debug.Log(turretPivot.localEulerAngles.x);

        /*if (fixedDirection > 0 && turretPivot.localEulerAngles.x > 180 &&
            turretPivot.localEulerAngles.x < (360 - maxPitchAngle))
        {
            turretPivot.localRotation = Quaternion.Euler(360 - maxPitchAngle + 1, 0, 0);
            return;
        }
        
        if (fixedDirection < 0 && turretPivot.localEulerAngles.x < 180 &&
            turretPivot.localEulerAngles.x > Mathf.Abs(minPitchAngle))
        {
            turretPivot.localRotation = Quaternion.Euler(Mathf.Abs(minPitchAngle) - 1, 0, 0);
            return;
        }*/
            
            
        /*{
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x < 360 - maxPitchAngle && turrenEulerAngles.x > 180)
                ? 360 - maxPitchAngle
                : turrenEulerAngles.x;
            turretPivot.localRotation = Quaternion.Euler(360 - maxPitchAngle, 0, 0);
        }
        else if (directionY < 0)
        {
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x > Mathf.Abs(minPitchAngle) && turrenEulerAngles.x < 180)
                ? Mathf.Abs(minPitchAngle)
                : turrenEulerAngles.x;
            turretPivot.localRotation = Quaternion.Euler(Mathf.Abs(minPitchAngle), 0, 0);
        }*/
        
        /*if (directionY > 0)
        {
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x < 360 - maxPitchAngle && turrenEulerAngles.x > 180)
                ? 360 - maxPitchAngle
                : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        }
        else if (directionY < 0)
        {
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x > Mathf.Abs(minPitchAngle) && turrenEulerAngles.x < 180)
                ? Mathf.Abs(minPitchAngle)
                : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        }*/
    }

    private void SmoothLerpPitch(float directionY, float movementSpeed)
    {
        if (directionY > 0 && (turretPivot.eulerAngles.x <= 180 || turretPivot.localEulerAngles.x >= 360 - maxPitchAngle))
        {
            // Calculate target rotation based on the angle
            float xRotation = turretPivot.eulerAngles.x - (maxPitchSpeed * Time.deltaTime);
            cannonPivotRotation = Quaternion.Euler(xRotation, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z);

            // Smoothly interpolate to the target rotation
            turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, cannonPivotRotation, rotationLerpTime);
        }
        
        if (directionY < 0 && (turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle) || turretPivot.eulerAngles.x > 180))
        {
            // Calculate target rotation based on the angle
            float xRotation = turretPivot.eulerAngles.x + (maxPitchSpeed * Time.deltaTime);
            cannonPivotRotation = Quaternion.Euler(xRotation, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z);

            // Smoothly interpolate to the target rotation
            turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, cannonPivotRotation, rotationLerpTime);
        }
    }

    private void SmoothDampPitch(float verticalDirection, float movementSpeed)
    {
        
        verticalDirection = isReversedPitch ? -verticalDirection : verticalDirection;
        
        float targetAngle = currentXAngle + verticalDirection * movementSpeed;

        //Debug.Log("target pitch: " + targetAngle);
        
        if (verticalDirection < 0 && targetAngle < -maxPitchAngle)
        {
            targetAngle = maxPitchAngle;
        }

        if (verticalDirection > 0 && targetAngle > -minPitchAngle)
        {
            targetAngle = -minPitchAngle;
        }

        currentXAngle = Mathf.SmoothDampAngle(currentXAngle, targetAngle, ref xVelocity, smoothTime);

        turretPivot.localRotation = Quaternion.Euler(currentXAngle,0 , 0);
        
        /*float targetAngle = currentXAngle + directionX * movementSpeed;
        currentXAngle = Mathf.SmoothDampAngle(currentXAngle, targetAngle, ref xVelocity, smoothTime);

        turretHead.rotation = Quaternion.Euler(currentXAngle,0 , 0);*/
        
        
        
        /*if (directionX > 0 && (turretPivot.eulerAngles.x <= 180 || turretPivot.localEulerAngles.x >= 360 - maxPitchAngle))
        {
            float targetAngle = currentXAngle + directionX * movementSpeed;
            currentXAngle = Mathf.SmoothDampAngle(currentXAngle, targetAngle, ref xVelocity, smoothTime);

            turretHead.rotation = Quaternion.Euler(currentXAngle,0 , 0);
        }
        
        if (directionX < 0 && (turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle) || turretPivot.eulerAngles.x > 180))
        {
            // Calculate target rotation based on the angle
            float xRotation = turretPivot.eulerAngles.x + (pitchSpeed * Time.deltaTime);
            cannonPivotRotation = Quaternion.Euler(xRotation, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z);

            // Smoothly interpolate to the target rotation
            turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, cannonPivotRotation, rotationLerpTime);
        }*/
    }

    private void SmoothLerpRotation(float direction, float movementSpeed)
    {
        float yRotation = turretHead.eulerAngles.y + direction * movementSpeed * Time.deltaTime;
        targetRotation = Quaternion.Euler(turretHead.eulerAngles.x, yRotation, turretHead.eulerAngles.z);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, rotationLerpTime);
    }

    private void SmoothDampRotate(float horizontralDirection = 0, float movementSpeed = 0)
    {
        float targetAngle = currentYAngle + horizontralDirection * movementSpeed;
        currentYAngle = Mathf.SmoothDampAngle(turretHead.eulerAngles.y, targetAngle, ref yVelocity, smoothTime);

        turretHead.rotation = Quaternion.Euler(0, currentYAngle, 0);
        
        /*float targetAngle = currentYAngle + horizontralDirection * movementSpeed;
        currentYAngle = Mathf.SmoothDampAngle(currentYAngle, targetAngle, ref yVelocity, smoothTime);

        turretHead.rotation = Quaternion.Euler(0, currentYAngle, 0);*/
    }

    public void CancelMovement()
    {
        //UpdateAnimator(Vector3.zero);
    }

    private (float rotationSpeed, float pitchSpeed) GetMovementSpeed(Vector2 screenTouchPos)
    {
        float touchesDistanceX = Mathf.Abs(screenTouchPos.x - startTouchPos.x);
        float touchDistanceY = Mathf.Abs(screenTouchPos.y - startTouchPos.y);
        float joystickSize = joystick.JoystickSize;

        var rotationSpeed = touchesDistanceX > joystickSize * .5f ? maxMovementSpeed : (touchesDistanceX / (joystickSize * .5f)) * maxMovementSpeed;
        var pitchSpeed = touchDistanceY > joystickSize * .5f ? maxPitchSpeed : (touchDistanceY / (joystickSize * .5f)) * maxPitchSpeed;

        return (rotationSpeed, pitchSpeed);
    }
    
    
    //Original method
    /*private (float rotationSpeed, float pitchSpeed) GetMovementSpeed(Vector2 screenTouchPos)
    {
        float touchesDistanceX = Vector2.Distance(screenTouchPos, startTouchPos.x);
        float joystickSize = joystick.JoystickSize;
        
        var rotSpeed = (touchesDistance / (joystickSize / 2)) * maxMovementSpeed;

        if (touchesDistance > joystickSize / 2)
        {
            return maxMovementSpeed;
        }
        
        

        return ((touchesDistance / (joystickSize / 2)) * maxMovementSpeed);

        //return (rotationSpeed: 2f, pitchSpeed: 3f);
    }*/

    public void ToggleMovement(bool enableMovement)
    {
        isMovementAllowed = enableMovement;
    }

    public void HideJoystick()
    {
        joystick.gameObject.SetActive(false);
    }

    public void ShowJoystick()
    {
        joystick.gameObject.SetActive(true);
    }
}