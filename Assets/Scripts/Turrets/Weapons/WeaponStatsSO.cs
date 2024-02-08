using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "TurboTowers/Weapons/New Weapon")]
public class WeaponStatsSO : ScriptableObject
{
    [SerializeField] private float[] baseRange = new float[3];
    [SerializeField] private int[] baseDamage = new int[3];
    [SerializeField] private float cooldown;
    
    public float GetBaseRange(int level)
    {
        return baseRange[level];
    }
    
    public int GetBaseDamage(int level)
    {
        return baseDamage[level];
    }
    
    public float GetCooldown()
    {
        return cooldown;
    }
}
