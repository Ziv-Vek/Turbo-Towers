using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DecalPainter : MonoBehaviour
{
    [SerializeField] private GameObject decalProjectorPrefab;

    public void PaintDecal(Vector3 contactPos)
    {
        Instantiate(decalProjectorPrefab, new Vector3(contactPos.x, 0.1f, contactPos.z), Quaternion.Euler(90, 0, 0));
    }
}