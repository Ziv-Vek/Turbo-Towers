using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboTowers.Turrets.Controls;

namespace TurboTowers.Turrets.Combat
{
    public class PlayerCannon: MonoBehaviour
    {
        [Space(10)]
        [Header("Projectile Shoot Out")]

        [Range(0.5f, 5.0f)]
        [SerializeField] private float rotationSpeed = 1;

        [Range(5.0f, 50.0f)]
        [SerializeField] private float BlastPower = 5;

        [Range(35.0f, 75.0f)]
        [SerializeField] private float sensitivity = 50f;

        [SerializeField] private Transform projectileSpawnPosition;

        [Tooltip("Object to be Rotated Vertically while Aiming (usually the base object)")]
        [SerializeField] private Transform RotatedVerticallyObj;

        [Tooltip("Object to be Rotated Horizontally while Aiming (usually the projectileSpawnPosition)")]
        [SerializeField] private Transform RotatedHorizontallyObj;

        [SerializeField] private GameObject projectile;
        
        private GameObject projectileTemp;

        bool canShoot = false;

        [Space(10)]
        [Header("Line Projection")]

        [SerializeField] private bool enableProjectionLine = true;

        [SerializeField] private LineRenderer lineRenderer;

        // Number of points on the line
        [SerializeField] private int numPoints = 50;

        // distance between those points on the line
        [SerializeField] private float timeBetweenPoints = 0.1f;

        [SerializeField] private float radius = .2f;

        [SerializeField] private Vector3 startingPosition;

        [SerializeField] private Transform contactPoint;

        Vector3 dir;

        [SerializeField] private LayerMask CollidableLayers;

        Collider[] colliders;

        private void Start()
        {
            canShoot = true;
        }

        private void Update()
        {
            if (Input.GetButton("Fire1") && canShoot)
            {
                float VericalRotation = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
                float HorizontalRotation = Input.GetAxis("Mouse Y") * -sensitivity * Time.deltaTime;

                RotatedVerticallyObj.rotation = Quaternion.Euler(RotatedVerticallyObj.rotation.eulerAngles.x, RotatedVerticallyObj.rotation.eulerAngles.y + VericalRotation * rotationSpeed, RotatedVerticallyObj.rotation.eulerAngles.z);
                RotatedHorizontallyObj.rotation = Quaternion.Euler(RotatedHorizontallyObj.rotation.eulerAngles.x, RotatedHorizontallyObj.rotation.eulerAngles.y, RotatedHorizontallyObj.rotation.eulerAngles.z + HorizontalRotation * rotationSpeed);

                if (enableProjectionLine)
                {
                    lineRenderer.enabled = true;
                    contactPoint.gameObject.SetActive(true);
                }
            }
            else if (Input.GetButtonUp("Fire1") && canShoot)
            {
                if (enableProjectionLine)
                {
                    lineRenderer.enabled = false;
                }

                canShoot = false;
                StartCoroutine(ShootProjectile());
            }

            if (!enableProjectionLine)
                return;

            lineRenderer.positionCount = (int)numPoints;
            List<Vector3> points = new List<Vector3>();
            if (lineRenderer.enabled)
                startingPosition = projectileSpawnPosition.position;
            Vector3 startingVelocity = projectileSpawnPosition.up * BlastPower;
            for (float t = 0; t < numPoints; t += timeBetweenPoints)
            {
                Vector3 newPoint = startingPosition + t * startingVelocity;
                newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t;

                points.Add(newPoint);

                colliders = Physics.OverlapSphere(newPoint, radius, CollidableLayers);
                if (colliders.Length > 0)
                {
                    lineRenderer.positionCount = points.Count;
                    contactPoint.transform.position = newPoint + new Vector3(0f, 1f, 0f);

                    break;
                }
            }
            lineRenderer.SetPositions(points.ToArray());
        }

        private IEnumerator ShootProjectile()
        {
            projectileTemp = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
            projectileTemp.transform.SetParent(projectileSpawnPosition);

            Rigidbody rb = projectileTemp.GetComponent<Rigidbody>();

            if(enableProjectionLine)
            {
                contactPoint.gameObject.SetActive(false);
                lineRenderer.enabled = false;
            }

            projectileTemp.transform.SetParent(null);

            yield return new WaitForSeconds(.1f);

            rb.isKinematic = false;
            rb.velocity = projectileSpawnPosition.up * BlastPower;

            yield return new WaitForSeconds(.35f);
            canShoot = true;
        }
    }
}