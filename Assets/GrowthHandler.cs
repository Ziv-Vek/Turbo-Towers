using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthHandler : MonoBehaviour
{
    [SerializeField] Transform headTransform;
    [SerializeField] Transform baseTransform;
    
    [SerializeField] GameObject bodyPartPrefab;
    
    List<GameObject> bodyParts = new List<GameObject>();
    
    private void Start()
    {
        var initialHealth = GetComponent<Health>().GetInitialHealth();
        
        
        
        for (int i = 0; i < initialHealth; i++)
        {
            var bodyPart = Instantiate(bodyPartPrefab, transform);
            
            if (i == 0)
            {
                var baseHighestYPos = baseTransform.GetComponent<Renderer>().bounds.max.y;
                var halfBodyPartHeight = bodyPart.GetComponent<Renderer>().bounds.size.y / 2;
                var yPos = baseHighestYPos + halfBodyPartHeight + 0.1f;
                bodyPart.transform.position = new Vector3(baseTransform.position.x, yPos, baseTransform.position.z);
                bodyPart.transform.rotation = baseTransform.rotation;
                
                continue;
            }
            
            bodyPart.transform.position = bodyParts[i - 1].transform.position;
            bodyPart
            
            
            
            
            
            
            /* heighest point of the base */
            /* + half of the body part height */
            /* + some gap
             this is the y position of the body part 
             x position is same as of the base,
             z position is same as of the base 
             
             
             but i need it to be suitable not only for the base 
             so for i = 0, it should be the same as the base
             
             for i > 0 it should be the same as the previous body part
             
             */
            
            
               
            
            
            bodyParts.Add(bodyPart);
        }
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

    private void GetNextYPos()
    {
        
    }

    public void Grow(int health)
    {
        var headRenderer = headTransform.GetComponent<Renderer>();
        if (headRenderer != null)
        {
            // headRenderer.
        }
    }
    
    public void Shrink(int damage)
    {
        Debug.Log("Taking damage: " + damage);
    }
}
