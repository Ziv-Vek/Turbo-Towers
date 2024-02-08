using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Common
{
    public class DeathVFXMover : MonoBehaviour
    {
        ParticleSystem[] particleSystems;
        [SerializeField] private Health health;
        
        private void OnEnable()
        {
            health.onDamageTaken += MoveVFX;
        }

        private void OnDisable()
        {
            health.onDamageTaken -= MoveVFX;
        }

        private void Start()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        private void MoveVFX(int damage, BodyPart targetBodyPart)
        {
            if (targetBodyPart == null)
            {
                Debug.LogError("Body part is null");
                return;
            }
            
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                //TODO: Check if this can be done for only one particle system
                particleSystem.transform.position = targetBodyPart.transform.position;
            }
        }

        /*private void OnApplicationQuit()
        {
            health.onDamageTaken -= MoveVFX;
        }*/
    }
}

