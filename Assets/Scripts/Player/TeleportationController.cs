using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationController : MonoBehaviour
{
    private Portal currentPortal;
    [SerializeField] private Portal portalPrefab;

    private void Start()
    {
        if (currentPortal == null)
        {
            var newPortal = Instantiate(portalPrefab, transform.position, portalPrefab.transform.rotation);
            currentPortal = newPortal.GetComponent<Portal>();
            currentPortal.DeactivatePortal();
        }
    }

    public void Teleport(Portal newPortal)
    {
        transform.SetPositionAndRotation(new Vector3(newPortal.transform.position.x, transform.position.y, newPortal.transform.position.z), Quaternion.identity);
        currentPortal.ActivatePortal();
        currentPortal = newPortal;
        currentPortal.DeactivatePortal();
    }
}
