using System;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private WeaponStatsSO weaponStats;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, weaponStats.GetBaseRange(0));
    }
}
