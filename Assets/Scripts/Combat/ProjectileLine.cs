using System;
using System.Collections;
using System.Collections.Generic;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Controls;
using TurboTowers.Turrets.Movement;
using UnityEngine;

namespace TurboTowers.Movement
{
    public class ProjectileLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField, Min(3)] private int lineSegments = 60;
        [SerializeField, Min(0.2f)] private float timeOfTheFlight = 5;
        [SerializeField] private Sprite lineSprite;
        [SerializeField] private float timeBetweenPoints = 0.1f;
        [SerializeField] private float radius = .3f;
        [SerializeField] private LayerMask CollidableLayers;
        [SerializeField] private Transform contactPoint;
        
        InputStyle inputStyle = InputStyle.First;
        Transform launchPoint;
        float launchVelocityMagnitude;
        Vector3 launchVelocity;

        private Collider[] colliders;

        private bool isProjeted = false;

        private void Start()
        {
            lineRenderer.startWidth = 0.5f;
            lineRenderer.endWidth = 0.5f;
            launchPoint = GetComponent<PlayerRotator>().GetLaunchPoint();
            launchVelocityMagnitude = GetComponent<PlayerAttacker>().GetShootingVelocityMagnitude();
            launchVelocity = GetComponent<PlayerAttacker>().GetShootingVelocity();
            inputStyle = GetComponent<TurboTowers.Turrets.Controls.PlayerController>().inputStyle;
        }

        private void OnEnable()
        {
            TurboTowers.Turrets.Controls.PlayerController.onTouchStarted += OnTouchStarted;
            TurboTowers.Turrets.Controls.PlayerController.onTouchPerformed += OnTouchPerformed;
            TurboTowers.Turrets.Controls.PlayerController.onTouchEnded += OnTouchEnded;
        }

        private void OnDisable()
        {
            TurboTowers.Turrets.Controls.PlayerController.onTouchStarted -= OnTouchStarted;
            TurboTowers.Turrets.Controls.PlayerController.onTouchPerformed -= OnTouchPerformed;
            TurboTowers.Turrets.Controls.PlayerController.onTouchEnded -= OnTouchEnded;
        }

        private void OnTouchEnded()
        {
            isProjeted = false;
            RemoveTrajectoryLine();
            GetComponent<PlayerAttacker>().Fire();
        }

        private void OnTouchPerformed(Vector2 obj)
        {
            UpdateLaunchParameters();
            //ShowTrajectoryLine(launchPoint.position, launchVelocity);
            RenderLine(launchPoint.position, launchVelocityMagnitude);
        }

        private void OnTouchStarted(Vector2 touchPos)
        {
            isProjeted = true;
            UpdateLaunchParameters();
            //ShowTrajectoryLine(launchPoint, launchVelocityMagnitude);
            RenderLine(launchPoint.position, launchVelocityMagnitude);
        }
        private void UpdateLaunchParameters()
        {
            launchPoint = GetComponent<PlayerRotator>().GetLaunchPoint();
            launchVelocityMagnitude = GetComponent<PlayerAttacker>().GetShootingVelocityMagnitude();
            launchVelocity = GetComponent<PlayerAttacker>().GetShootingVelocity();
        }

        private void FixedUpdate()
        {
            if (inputStyle == InputStyle.Fourth && isProjeted)
            {
                //ShowTrajectoryLine(launchPoint, launchVelocityMagnitude);
                RenderLine(launchPoint.position, launchVelocityMagnitude);
            }
        }

        public void RenderLine(Vector3 spawnPosition, float startingVelocityMagnitude)
        {
            lineRenderer.positionCount = lineSegments;
            List<Vector3> points = new List<Vector3>();
            Vector3 velocityVector = launchVelocity.normalized * startingVelocityMagnitude;
            
            for (float t = 0; t < lineSegments; t += timeBetweenPoints)
            {
                Vector3 displacement = velocityVector * t + 0.5f * Physics.gravity * t * t;
                Vector3 newPoint = spawnPosition + displacement;

                points.Add(newPoint);

                colliders = Physics.OverlapSphere(newPoint, radius, CollidableLayers);
                if (colliders.Length > 0)
                {
                    lineRenderer.positionCount = points.Count;
                    //contactPoint.transform.position = newPoint + new Vector3(0f, 1f, 0f);
                    contactPoint.position = newPoint;

                    break;
                }
            }

            lineRenderer.SetPositions(points.ToArray());
        }

        public void ShowTrajectoryLine(Transform startPoint, float startVelocity)
        {
            float timeStep = timeOfTheFlight / lineSegments;
            Vector3[] lineRendererPoints = CalculateTrajectoryLine(startPoint.position, startPoint.forward * launchVelocityMagnitude, timeStep);

            lineRenderer.positionCount = lineSegments;
            lineRenderer.SetPositions(lineRendererPoints);
            //lineRenderer.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        public void RemoveTrajectoryLine()
        {
            lineRenderer.positionCount = 0;
        }

        private Vector3[] CalculateTrajectoryLine(Vector3 startPoint, Vector3 startVelocity, float timeStep)
        {
            Vector3[] lineRendererPoints = new Vector3[lineSegments];

            lineRendererPoints[0] = startPoint;
            for (int i = 1; i < lineSegments; i++)
            {
                float timeOffset = timeStep * i;
                Vector3 progressBeforeGravity = startVelocity * timeOffset;
                Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * timeOffset * timeOffset;
                Vector3 newPosition = startPoint + progressBeforeGravity - gravityOffset;
                lineRendererPoints[i] = newPosition;
            }

            return lineRendererPoints;
        }
    }
}