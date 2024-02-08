using System;
using UnityEngine;

public class PortalsManager : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;

    private static PortalsManager instance;

    public static PortalsManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private static void SetupInstance()
    {
        instance = FindObjectOfType<PortalsManager>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = "PortalsManager";
            instance = gameObj.AddComponent<PortalsManager>();
            DontDestroyOnLoad(gameObj);
        }
    }

    public void SpawnPortal(Vector3 position)
    {
        Instantiate(portalPrefab, position, Quaternion.identity);
    }
}