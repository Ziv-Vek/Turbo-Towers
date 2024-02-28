using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Controls;
using UnityEditor;

namespace TurboTowers.Turrets.Movement
{
    public class PlayerRotator : MonoBehaviour
    {
        #region Variables
        [Space]
        [SerializeField] public Transform turretHead;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Transform turretPivot;
        [SerializeField] private float pitchSpeed = 15f;
        [SerializeField, Range(-90, 90)] private float maxPitchAngle = 60f;
        [SerializeField, Range(-45, 45)] private float minPitchAngle = 0;
        [SerializeField] private HoldClickableButton powerBtn;

        private InputStyle inputStyle = InputStyle.First;

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
        private Quaternion cannonPivotRotation;

        #endregion



        #region Unity Events

        private void Start()
        {
            // Initialize target rotation to the current rotation of the turret head
            targetRotation = turretHead.rotation;
            cannonPivotRotation = turretPivot.rotation;
        }

        private void OnEnable()
        {
            TurboTowers.Turrets.Controls.PlayerController.onHorizontalTouchDrag += HorizontalTouchDragHandler;
            TurboTowers.Turrets.Controls.PlayerController.onVerticalTouchDrag += VerticalTouchDragHandler;

            GetComponent<PlayerAttacker>().OnTurretPowering += DisableRotation;
            GetComponent<PlayerAttacker>().OnTurretFired += EnableRotation;
            GetComponent<PlayerSlideAttacker>().OnTurretPowering += DisableRotation;
            GetComponent<PlayerSlideAttacker>().OnTurretFired += EnableRotation;

            StartCoroutine(HandleInputListeners());
        }

        private void OnDisable()
        {
            TurboTowers.Turrets.Controls.PlayerController.onHorizontalTouchDrag -= HorizontalTouchDragHandler;
            TurboTowers.Turrets.Controls.PlayerController.onVerticalTouchDrag -= VerticalTouchDragHandler;

            GetComponent<PlayerAttacker>().OnTurretPowering -= DisableRotation;
            GetComponent<PlayerAttacker>().OnTurretFired -= EnableRotation;
            GetComponent<PlayerSlideAttacker>().OnTurretPowering -= DisableRotation;
            GetComponent<PlayerSlideAttacker>().OnTurretFired -= EnableRotation;

            InputSwitchHandler.Instance.OnInputStyleSelect -= InputStyleSelectHandler;
        }
        #endregion

        private IEnumerator HandleInputListeners()
        {
            if (!InputSwitchHandler.Instance) yield return null;

            InputSwitchHandler.Instance.OnInputStyleSelect += InputStyleSelectHandler;
        }

        private void InputStyleSelectHandler(InputStyle inputStyle)
        {
            this.inputStyle = inputStyle;

            if (this.inputStyle == InputStyle.Second)
            {
                SetTurretToFixedRotation();
            }
        }

        private void HorizontalTouchDragHandler(int touchDragDirection)
        {
            if (!isHorizontalRotationActive) return;

            RotateTurretHead(touchDragDirection);
        }

        public void RotateTurretHead(float direction)
        {
            if (!isHorizontalRotationActive) return;

            // implements smooth rotation, or lerp rotation, or normal rotation
            //TODO: delete two other implementations not used

            if (isSmoothImplemented)
            {
                float targetAngle = currentYAngle + direction * rotationSpeed;
                currentYAngle = Mathf.SmoothDampAngle(currentYAngle, targetAngle, ref yVelocity, smoothTime);

                turretHead.rotation = Quaternion.Euler(0, currentYAngle, 0);
            }
            else if(isLerpImplemented)
            {
                /*targetRotation = Quaternion.Euler(0, angle, 0);
                turretHead.rotation = Quaternion.Lerp(turretHead.rotation, targetRotation, rotationLerpTime);*/


                // Calculate target rotation based on the angle
                float yRotation = turretHead.eulerAngles.y + direction * rotationSpeed * Time.deltaTime;
                targetRotation = Quaternion.Euler(turretHead.eulerAngles.x, yRotation, turretHead.eulerAngles.z);

                // Smoothly interpolate to the target rotation
                turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, rotationLerpTime);
            }
            else
            {
                turretHead.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime * direction));
            }

            //var targetPivotRot = new Vector3(turretPivot.eulerAngles.x, 0, 0);
            //turretPivot.rotation = Quaternion.Euler(targetPivotRot);
        }

        private void VerticalTouchDragHandler(int touchDragDirection)
        {
            if (inputStyle != InputStyle.First) return;

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
            if (turretPivot.eulerAngles.x <= 180 || turretPivot.localEulerAngles.x >= 360 - maxPitchAngle)
            {
                // Calculate target rotation based on the angle
                float xRotation = turretPivot.eulerAngles.x - (pitchSpeed * Time.deltaTime);
                cannonPivotRotation = Quaternion.Euler(xRotation, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z);
                
                // Smoothly interpolate to the target rotation
                turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, cannonPivotRotation, rotationLerpTime);
            }
            
            // Clamp rotation
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x < 360 - maxPitchAngle && turrenEulerAngles.x > 180) ? 360 - maxPitchAngle : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        }

        public void LowerTurretPivot()
        {
            if (turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle) || turretPivot.eulerAngles.x > 180)
            {
                 // Calculate target rotation based on the angle
                 float xRotation = turretPivot.eulerAngles.x + (pitchSpeed * Time.deltaTime);
                 cannonPivotRotation = Quaternion.Euler(xRotation, turretPivot.eulerAngles.y, turretPivot.eulerAngles.z);

                 // Smoothly interpolate to the target rotation
                 turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, cannonPivotRotation, rotationLerpTime);
            }
            
            // Clamp rotation
            Vector3 turrenEulerAngles = turretPivot.rotation.eulerAngles;
            turrenEulerAngles.x = (turrenEulerAngles.x > Mathf.Abs(minPitchAngle) && turrenEulerAngles.x < 180) ? Mathf.Abs(minPitchAngle) : turrenEulerAngles.x;
            turretPivot.rotation = Quaternion.Euler(turrenEulerAngles);
        }

        private void SetTurretToFixedRotation()
        {
            // turretPivot.Rotate(-25, 0, 0);
            
            //turretPivot.RotateAround(turretPivot.localPosition, Vector3.right, -25);
            
            
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
}
