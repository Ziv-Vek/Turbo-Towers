using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private LineRenderer centerLine;
    [SerializeField] private LineRenderer rightLine;
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private float lineSpacing = 3f;
    [SerializeField] private float boxColliderCenterDistance = 50f;
    [SerializeField] private Vector3 boxColliderSize = new Vector3(5, 10, 50);
    [SerializeField] private Material proximityTargetMat;
    [SerializeField] private Material targetMat;
    
    private bool started = false;

    private void Start()
    {
        started = true;
    }

    private void FixedUpdate()
    {
        centerLine.SetPositions(new Vector3[] { head.position, head.up * 100 });
        rightLine.SetPositions(new Vector3[] { head.position + head.right * lineSpacing, head.up * 100 + head.right * lineSpacing});
        leftLine.SetPositions(new Vector3[] { head.position + head.right * -lineSpacing, head.up * 100  + head.right * -lineSpacing});

        var targetsInFront = Physics.OverlapBox(head.position + head.up * boxColliderCenterDistance, boxColliderSize,Quaternion.identity,
            LayerMask.GetMask("Targetable"));


        foreach (var target in targetsInFront)
        {
            
        }
        
    }

    private void OnDrawGizmos()
    {
        if (started)
        Gizmos.DrawWireCube(head.position + head.up * boxColliderCenterDistance, boxColliderSize);
    }

}





/*
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform enemy; // Reference to the enemy's transform
    public float rotationSpeed = 5f; // Adjust the rotation speed as needed

    void Update()
    {
        AimAtEnemy();
    }

    void AimAtEnemy()
    {
        // Ensure that the enemy reference is not null
        if (enemy != null)
        {
            // Get the screen position of the enemy in world space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.position);

            // Check if the enemy is close to the center vertical line
            if (Mathf.Abs(screenPos.x - Screen.width / 2) < Screen.width * 0.1f)
            {
                // Calculate the direction to the enemy
                Vector3 direction = enemy.position - transform.position;
                direction.y = 0f; // Ensure the rotation is only in the horizontal plane

                // Rotate the player to aim at the enemy
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}



using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 5f; // Adjust the rotation speed as needed
    public float detectionRadius = 10f; // Adjust the detection radius as needed

    void Update()
    {
        AimAtClosestEnemy();
    }

    void AimAtClosestEnemy()
    {
        // Find all enemies within the detection radius
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));

        Transform closestEnemy = null;
        float minDistanceToCenter = float.MaxValue;

        // Iterate through each enemy to find the closest one to the center vertical line
        foreach (var enemyCollider in enemies)
        {
            Transform enemy = enemyCollider.transform;

            // Get the screen position of the enemy in world space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.position);

            // Check if the enemy is close to the center vertical line
            if (Mathf.Abs(screenPos.x - Screen.width / 2) < Screen.width * 0.1f)
            {
                // Calculate the distance to the center vertical line
                float distanceToCenter = Mathf.Abs(screenPos.x - Screen.width / 2);

                // Check if this enemy is closer to the center and closer overall
                if (distanceToCenter < minDistanceToCenter)
                {
                    minDistanceToCenter = distanceToCenter;
                    closestEnemy = enemy;
                }
            }
        }

        // Rotate towards the closest enemy
        if (closestEnemy != null)
        {
            Vector3 direction = closestEnemy.position - transform.position;
            direction.y = 0f; // Ensure the rotation is only in the horizontal plane

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
*/


