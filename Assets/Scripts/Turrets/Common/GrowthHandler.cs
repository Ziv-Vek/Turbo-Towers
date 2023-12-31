using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GrowthHandler : MonoBehaviour
{
    [SerializeField] Transform headTransform;
    [SerializeField] Transform baseTransform;
    
    [SerializeField] GameObject bodyPartPrefab;
    
    List<GameObject> bodyParts = new List<GameObject>();

    const float GAP_BETWEEN_BODY_PARTS = 0.1f;
    
    private void Start()
    {
        var initialHealth = GetComponent<Health>().GetInitialHealth();

        var bodyPartHeight = bodyPartPrefab.GetComponentInChildren<MeshRenderer>().bounds.size.y;

        for (int i = 0; i < initialHealth; i++)
        {
            var bodyPart = Instantiate(bodyPartPrefab, transform);
            
            // place bodypart based on the base part
            if (i == 0)
            {
                var baseHighestYPos = baseTransform.GetComponent<MeshRenderer>().bounds.max.y;
                var yPos = baseHighestYPos + (bodyPartHeight / 2) + GAP_BETWEEN_BODY_PARTS;
                bodyPart.transform.position = new Vector3(baseTransform.position.x, yPos, baseTransform.position.z);
                bodyPart.transform.rotation = baseTransform.rotation;

                bodyParts.Add(bodyPart);

                continue;
            }
            
            Vector3 prevBodyPos = bodyParts[i - 1].transform.position;
            bodyPart.transform.position = new Vector3 (prevBodyPos.x, prevBodyPos.y + bodyPartHeight + GAP_BETWEEN_BODY_PARTS, prevBodyPos.z);
            
            bodyParts.Add(bodyPart);
        }

        var lastBodyPart = bodyParts[bodyParts.Count - 1];

        headTransform.position = new Vector3(lastBodyPart.transform.position.x, lastBodyPart.transform.position.y +
                 bodyPartHeight / 2 + headTransform.GetComponent<MeshRenderer>().bounds.extents.y + GAP_BETWEEN_BODY_PARTS, lastBodyPart.transform.position.z);
    }

    private void OnEnable()
    {
        GetComponent<Health>().onDamageTaken += Shrink;
        GetComponent<Health>().onHealthGained += Grow;
    }

    private void OnDisable()
    {
        GetComponent<Health>().onDamageTaken -= Shrink;
        GetComponent<Health>().onHealthGained -= Grow;
    }

    public void Grow(int health)
    {
        Debug.Log("Gaining health: " + health);

        var headRenderer = headTransform.GetComponent<Renderer>();
        if (headRenderer != null)
        {
            // headRenderer.
        }
    }
    
    public void Shrink(int damage)
    {
        Debug.Log("Taking damage: " + damage);

        var headRenderer = headTransform.GetComponent<MeshRenderer>();

        if (headRenderer != null)
        {
            // headRenderer
        }
    }
}
