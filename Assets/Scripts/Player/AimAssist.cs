using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    
    [SerializeField] private Transform head;
    [SerializeField] private LineRenderer centerLine;
    [SerializeField] private LineRenderer rightLine;
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private float lineSpacing = 3f;
    [SerializeField] private float boxColliderCenterDistance = 200f;
    [SerializeField] private Vector3 boxColliderSize = new Vector3(5, 10, 50);
    
    private bool started = false;
    private List<Collider> targetsInFOV = new List<Collider>();
    private List<Collider> previousTargetsInFOV = new List<Collider>();

    public GameObject dummyCube;

    private float cooldownTimer = 0f;
    private const float AUTO_AIM_COOLDOWN_TIME = 3f;

    private void Start()
    {
        cooldownTimer = AUTO_AIM_COOLDOWN_TIME;
        started = true;
    }

    private void FixedUpdate()
    {
        centerLine.SetPositions(new Vector3[] { head.position, head.up * 100 });
        rightLine.SetPositions(new Vector3[] { head.position + head.right * lineSpacing, head.up * 100 + head.right * lineSpacing});
        leftLine.SetPositions(new Vector3[] { head.position + head.right * -lineSpacing, head.up * 100  + head.right * -lineSpacing});

        var detectedObjects = Physics.OverlapBox(head.position + head.up * boxColliderCenterDistance, boxColliderSize,head.rotation,
            LayerMask.GetMask("Targetable"));

        // Remove targets that are no longer in the detectedObjects array
        previousTargetsInFOV = targetsInFOV.Where(target => !detectedObjects.Contains(target as Collider)).ToList();
        
        // foreach (var target in targetsInFOV)
        // {
        //     if (!detectedObjects.Contains(target as Collider))
        //     {
        //         if (target.TryGetComponent(out ITargetable targetable))
        //         {
        //             targetable.UnPaintTargeted();
        //         }
        //         //targetsInFOV.Remove(target);
        //         
        //     }
        // }
        
        // Remove targets that are no longer in the detectedObjects array
        targetsInFOV = targetsInFOV.Where(target => detectedObjects.Contains(target as Collider)).ToList();

        
        
        // Paint and Unpaint targets
        foreach (var detectedObject in detectedObjects)
        {
            // If the target is not already in the list, add it and paint it
            if (!targetsInFOV.Contains(detectedObject) && detectedObject.TryGetComponent(out ITargetable targetable))
            {
                targetsInFOV.Add(detectedObject);
                targetable.PaintTargeted();
            }
        }

        if (targetsInFOV.Count > 0)
        {
            var closestTarget = FindClosestTarget();
            
            if(cooldownTimer > 0f)
            {
                AutoAimAtTarget(closestTarget);
                cooldownTimer -= Time.fixedDeltaTime;
            }
        }

        if (cooldownTimer < 0f)
        {
            StartCooldown();
        }
        

        // Unpaint targets that are no longer in the FOV
        

        
        /*foreach (var detectedObject in detectedObjects)
        {
            if (detectedObject.TryGetComponent(out Collider target))
            {
                targetsInFOV.Add(target);
                target.GetComponent<ITargetable>().PaintTargeted();
            }
        }
        
        foreach (var target in targetsInFOV)
        {
            if (!detectedObjects.Contains(target as Collider))
            {
                targetsInFOV.Remove(target);
                target.GetComponent<ITargetable>().UnPaintTargeted();
            }
        }*/
        
        
        
        dummyCube.transform.SetPositionAndRotation(head.position + head.up * boxColliderCenterDistance, head.rotation);
        dummyCube.transform.localScale = (boxColliderSize * 2);
    }

    private Collider FindClosestTarget()
    {
        Collider closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider target in targetsInFOV)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(gameObject.transform.position, target.ClosestPoint(gameObject.transform.position));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
        }
        
        return closestTarget;
    }

    void AutoAimAtTarget(Collider closestTarget)
    {
        Vector3 directionToTarget = (closestTarget.transform.position - GetComponent<PlayerRotator>().turretHead.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        GetComponent<PlayerRotator>().turretHead.rotation = Quaternion.Lerp(
            GetComponent<PlayerRotator>().turretHead.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    }

    private void StartCooldown()
    {
        cooldownTimer = AUTO_AIM_COOLDOWN_TIME;
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


