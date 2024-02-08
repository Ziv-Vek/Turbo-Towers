using UnityEngine;

namespace TurboTowers.Turrets.Combat
{
    public class TeleportationController : MonoBehaviour
    {
        private Portal currentPortal;
        [SerializeField] private Portal portalPrefab;
        [Tooltip("If true, a portal will be reactivated in the previous position of the player after teleportation.")]
        [SerializeField] private bool isReactivatingPortals = false;

        private void Start()
        {
            if (currentPortal == null)
            {
                var newPortal = Instantiate(portalPrefab, transform.position, portalPrefab.transform.rotation);
                currentPortal = newPortal.GetComponent<Portal>();
                currentPortal.DeactivatePortal();
            }
        }

        public void Teleport(Portal designatedPortal)
        {
            transform.SetPositionAndRotation(new Vector3(designatedPortal.transform.position.x, designatedPortal.transform.position.z, designatedPortal.transform.position.z), Quaternion.identity);
            
            if (isReactivatingPortals)
            {
                currentPortal.ActivatePortal();
            }
            
            currentPortal = designatedPortal;
            currentPortal.DeactivatePortal();
        }
    }
}
