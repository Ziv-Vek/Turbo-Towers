using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullseye : MonoBehaviour, ITargetable
{
    public int GetCurrentHealth()
    {
        return 1;
    }

    public bool IsAlive()
    {
        return true;
    }
}
