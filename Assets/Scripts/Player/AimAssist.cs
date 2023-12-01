using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [SerializeField] private Transform head;


    private void Start()
    {
        var line = head.GetComponent<LineRenderer>();

        line.SetPositions(new Vector3[] { head.position, new Vector3(head.position.x, head.position.y, head.position.z + 100) });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(head.position, head.forward * 100f);
    }
}
