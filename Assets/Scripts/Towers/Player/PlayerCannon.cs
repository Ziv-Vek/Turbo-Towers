using System;
using System.Collections;
using System.Collections.Generic;
using TurboTowers.Core;
using TurboTowers.Turrets.Common;
using UnityEngine;
using TurboTowers.Turrets.Controls;
using UnityEngine.InputSystem;

namespace TurboTowers.Turrets.Combat
{
    public class PlayerCannon: MonoBehaviour
    {
        [Header("Touch Config:")]
        [Space(10)]
        [SerializeField] private float touchDragDeadZone = 5f;
        [SerializeField] private float minVerticalDelta = 2f;
        
        [Space(10)]
        [Header("Projectile Shoot Out")]

        [Range(0.5f, 5.0f)]
        [SerializeField] private float rotationSpeed = 1;

        [Range(5.0f, 50.0f)]
        [SerializeField] private float BlastPower = 5;

        [Range(35.0f, 75.0f)]
        [SerializeField] private float sensitivity = 50f;

        [SerializeField] private Transform projectileSpawnPosition;

        [Tooltip("Object to be Rotated Vertically while Aiming (usually the base object)")]
        [SerializeField] private Transform RotatedVerticallyObj;

        [Tooltip("Object to be Rotated Horizontally while Aiming (usually the projectileSpawnPosition)")]
        [SerializeField] private Transform RotatedHorizontallyObj;

        [SerializeField] private GameObject projectile;
        
        private GameObject projectileTemp;

        bool canShoot = false;

        [Space(10)]
        [Header("Line Projection")]

        [SerializeField] private bool enableProjectionLine = true;

        [SerializeField] private LineRenderer lineRenderer;

        // Number of points on the line
        [SerializeField] private int numPoints = 50;

        // distance between those points on the line
        [SerializeField] private float timeBetweenPoints = 0.1f;

        [SerializeField] private float radius = .2f;

        [SerializeField] private Vector3 startingPosition;

        [SerializeField] private Transform contactPoint;

        Vector3 dir;

        [SerializeField] private LayerMask CollidableLayers;

        Collider[] colliders;
        
        // Touch variables:
        private bool isTouching = false;
        private bool isTowerControlEnabled = true;
        private Vector2 previousTouchPos;
        
        // Touch cached ref:
        private Coroutine inputDragCoroutine;
        
        private PlayerInput playerInput;
        private InputAction touchPositionAction;
        private InputAction touchPressAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            touchPressAction = playerInput.actions.FindAction("TouchPress");
            touchPositionAction = playerInput.actions["TouchPosition"];
        }

        private void OnEnable()
        {
            touchPressAction.performed += TouchPressed;
            touchPressAction.canceled += TouchCanceled;
            touchPressAction.started += TouchStarted;
            
            //GetComponentInParent<Health>().OnDeath += OnPlayerDeath;
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            touchPressAction.performed += TouchPressed;
            touchPressAction.canceled -= TouchCanceled;
            touchPressAction.started -= TouchStarted;
             
            //GetComponentInParent<Health>().OnDeath -= OnPlayerDeath;
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Start()
        {
            canShoot = true;
        }
        
        private void OnGameStateChanged(GameState newGameState)
        {
            isTowerControlEnabled = newGameState is GameState.InGame or GameState.InGameBoss;
        }
        
        private void TouchStarted(InputAction.CallbackContext obj)
        {
            //Debug.Log("started");
        }
        
        void TouchPressed(InputAction.CallbackContext context)
        {
            Debug.Log("touch started");
            if (!isTowerControlEnabled) return;
            
            Vector2 startTouchPos = touchPositionAction.ReadValue<Vector2>();
            previousTouchPos = startTouchPos;
            
            isTouching = true;
            
            // onTouchStarted?.Invoke(startTouchPos);
            
            StartCoroutine(TouchDrag(startTouchPos));
        }
        
        void TouchCanceled(InputAction.CallbackContext context)
        {
            if (!isTowerControlEnabled) return;
            
            isTouching = false;
            //onTouchEnded?.Invoke();

            if (canShoot)
            {
                if (enableProjectionLine)
                {
                    lineRenderer.enabled = false;
                }

                canShoot = false;
                StartCoroutine(ShootProjectile());
            }
            
        }
        
        IEnumerator TouchDrag(Vector2 startTouchPos)
        {
            float verticalAngle;
            float horizontalAngle;
            
            var currentTouchPos = new Vector2();
            
            while (isTouching)
            {
                currentTouchPos = touchPositionAction.ReadValue<Vector2>();

                int dragDirectionX = 0;
                int dragDirectionY = 0;
                    
                if (currentTouchPos.x - previousTouchPos.x < touchDragDeadZone)
                {
                    dragDirectionX = 0;
                }
                else
                {
                    dragDirectionX = currentTouchPos.x > previousTouchPos.x ? 1 : -1;
                }
                
                if (currentTouchPos.y - previousTouchPos.y < touchDragDeadZone)
                {
                    dragDirectionY = 0;
                }
                else
                {
                    dragDirectionY = currentTouchPos.y > previousTouchPos.y ? 1 : -1;
                }

                //Debug.Log(dragDirectionX == 1 ? "Right" : "Left");
                // Debug.Log(dragDirectionY == 1 ? "Up" : "Down");
                
                
                float VericalRotation = dragDirectionX * sensitivity * Time.deltaTime;
                float HorizontalRotation = dragDirectionY * -sensitivity * Time.deltaTime;

                RotatedVerticallyObj.rotation = Quaternion.Euler(RotatedVerticallyObj.rotation.eulerAngles.x, RotatedVerticallyObj.rotation.eulerAngles.y + VericalRotation * rotationSpeed, RotatedVerticallyObj.rotation.eulerAngles.z);
                RotatedHorizontallyObj.rotation = Quaternion.Euler(RotatedHorizontallyObj.rotation.eulerAngles.x, RotatedHorizontallyObj.rotation.eulerAngles.y, RotatedHorizontallyObj.rotation.eulerAngles.z + HorizontalRotation * rotationSpeed);

                if (enableProjectionLine)
                {
                    lineRenderer.enabled = true;
                    contactPoint.gameObject.SetActive(true);
                }
                
                
                
                //(bool isOutsideOfDeadzone, bool isVerticalDeltaEnough) = IsOutsideMinimalDrag(startTouchPos, currentTouchPos);
                 
                //horizontalAngle = Mathf.Atan2(currentTouchPos.y - startTouchPos.y, currentTouchPos.x - startTouchPos.x) * 180 / Mathf.PI;
                //horizontalAngle = Mathf.Atan2(currentTouchPos.y - previousTouchPos.y, currentTouchPos.x - previousTouchPos.x) * 180 / Mathf.PI;
                //Debug.Log(angle.ToString());
                 
                 
                // Calculate direction vector from previous to current touch position
                //Vector2 direction = currentTouchPos - previousTouchPos;

                // Normalize the direction
                //direction.Normalize();
                
                // Calculate the angle in radians and then convert to degrees
                //verticalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                // Normalize angle to 0 to 360 degrees if needed
                //if (angle < 0) angle += 360;
                 
                 
                /*if (isVerticalDeltaEnough)
                    HandleVerticalDragActionByTouchAngle(verticalAngle, currentTouchPos.y);
                if (isOutsideOfDeadzone)
                    //HandleHorizontalDragActionByTouchDelta(direction.x);
                    HandelHorizontalActionByTouchAngle(horizontalAngle);*/

                previousTouchPos = currentTouchPos;
                 
                yield return null;
            }
        }
        
        private (bool, bool) IsOutsideMinimalDrag(Vector2 startTouchPos, Vector2 currentTouchPos)
        {
            var isOutsideDeadzone = Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone;
            var isVerticalDeltaEnough = Mathf.Abs(Mathf.Abs(currentTouchPos.y) - Mathf.Abs(previousTouchPos.y)) > minVerticalDelta;

            return (isOutsideDeadzone, isVerticalDeltaEnough);
        }
        
        private void OnPlayerDeath()
        {
            GameManager.Instance.SetGameState(GameState.Defeat);
        }

        private void Update()
        {
            if (!enableProjectionLine)
                return;

            lineRenderer.positionCount = (int)numPoints;
            List<Vector3> points = new List<Vector3>();
            if (lineRenderer.enabled)
                startingPosition = projectileSpawnPosition.position;
            Vector3 startingVelocity = projectileSpawnPosition.up * BlastPower;
            for (float t = 0; t < numPoints; t += timeBetweenPoints)
            {
                Vector3 newPoint = startingPosition + t * startingVelocity;
                newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t;

                points.Add(newPoint);

                colliders = Physics.OverlapSphere(newPoint, radius, CollidableLayers);
                if (colliders.Length > 0)
                {
                    lineRenderer.positionCount = points.Count;
                    contactPoint.transform.position = newPoint + new Vector3(0f, 1f, 0f);

                    break;
                }
            }
            lineRenderer.SetPositions(points.ToArray());
        }

        private IEnumerator ShootProjectile()
        {
            projectileTemp = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
            projectileTemp.transform.SetParent(projectileSpawnPosition);

            Rigidbody rb = projectileTemp.GetComponent<Rigidbody>();

            if(enableProjectionLine)
            {
                contactPoint.gameObject.SetActive(false);
                lineRenderer.enabled = false;
            }

            projectileTemp.transform.SetParent(null);

            yield return new WaitForSeconds(.1f);

            rb.isKinematic = false;
            rb.velocity = projectileSpawnPosition.up * BlastPower;

            yield return new WaitForSeconds(.35f);
            canShoot = true;
        }
    }
}