using System.Collections;
using System.Collections.Generic;
using TurboTowers.Movement;
using UnityEngine;

namespace TurboTowers.Turrets.Combat
{
    public class TeleportationController : MonoBehaviour
    {
        private Portal currentPortal = null;
        [SerializeField] private Portal portalPrefab;
        [Tooltip("If true, a portal will be reactivated in the previous position of the player after teleportation.")]
        [SerializeField] private bool isReactivatingPortals = false;

        [SerializeField] private Transform baseTransform;
        [SerializeField] private float delayForTeleportationAfterKill = 0.5f;
        [SerializeField] private float delayForTeleportationToPortal = 0.5f;

        private void Start()
        {
            if (currentPortal == null)
            {
                var newPortal = Instantiate(portalPrefab, transform.position, portalPrefab.transform.rotation);
                currentPortal = newPortal.GetComponent<Portal>();
                currentPortal.DeactivatePortal();
            }
        }
        
        public IEnumerator Teleport(Portal designatedPortal, bool isTeleportingByEnemyKilled)
        {
            Vector3 targetPosition = designatedPortal.transform.position;
            
            if (isReactivatingPortals)
            {
                if (currentPortal == null)
                {
                    PortalsManager.Instance.SpawnPortal(baseTransform.position);
                }
                else
                {
                    currentPortal.ActivatePortal();    
                }
            }

            yield return new WaitForSeconds(isTeleportingByEnemyKilled ? delayForTeleportationAfterKill : delayForTeleportationToPortal);

            transform.SetPositionAndRotation(targetPosition, Quaternion.identity);
            
            currentPortal = designatedPortal;
            currentPortal.DeactivatePortal();
            
            yield return null;
        }
    }
}
