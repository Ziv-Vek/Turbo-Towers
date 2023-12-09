using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private Portal currentPortal;
    [SerializeField] private GameObject portalPrefab;

    private void Start()
    {
        if (currentPortal == null)
        {
            var newPortal = Instantiate(portalPrefab, transform.position, portalPrefab.transform.rotation);
            currentPortal = newPortal.GetComponent<Portal>();
            currentPortal.DeactivatePortal();
        }
    }

    public void TeleporHandler(Teleport attacker, Portal newPortal)
    {
        currentPortal = newPortal;
        transform.SetPositionAndRotation(new Vector3(newPortal.transform.position.x, transform.position.y, newPortal.transform.position.z), Quaternion.identity);
    }
}
