using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFaider : MonoBehaviour
{
    [SerializeField] private float disappearTimer = 5f;
    
    private float timer;

    private DecalProjector decalProjector;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
    }

    private void Start()
    {
        timer = disappearTimer;
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0) Destroy(this);

        FadeOutDecal();
    }

    private void FadeOutDecal()
    {
        decalProjector.fadeFactor = (timer / disappearTimer);
    }
}
