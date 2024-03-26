using JetBrains.Annotations;
using UnityEngine;

namespace TurboTowers.Turrets.Combat
{
    public interface IActionable
    {
        public void PerformActionOnHit([CanBeNull] TeleportationController teleported);
    }        
}
