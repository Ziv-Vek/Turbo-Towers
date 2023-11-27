using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DecalPainter : MonoBehaviour
{
    [SerializeField] private DecalTextureData[] decalData;
    [SerializeField] private GameObject decalProjectorPrefab;
    [SerializeField] private int selectedDecalIndex;
    [SerializeField] private Image decalImage;

    private Material[] decalMaterials;

    private void Start()
    {
        decalMaterials = new Material[decalData.Length];
        selectedDecalIndex = 0;
        foreach (Image image in FindObjectsOfType<Image>())
        {
            if (image.CompareTag("Decal"))
            {
                decalImage = image;
                break;
            }
            
        }

        decalImage.sprite = decalData[selectedDecalIndex].sprite;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedDecalIndex++;
            if (selectedDecalIndex >= decalData.Length)
                selectedDecalIndex = 0;
            decalImage.sprite = decalData[selectedDecalIndex].sprite;
        }
    }
}

/// <summary>
/// Decal data to store sprite and size
/// </summary>
[Serializable]
public class DecalTextureData
{
    public Sprite sprite;
    public Vector3 size;
}
