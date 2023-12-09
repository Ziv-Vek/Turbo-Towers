using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Teleport(Vector3 portalPosition)
    {
        transform.SetPositionAndRotation(new Vector3(portalPosition.x, transform.position.y, portalPosition.z), Quaternion.identity);
    }
}
