using JetBrains.Annotations;
using UnityEngine;

public interface IActionable
{
        public void PerformActionOnHit([CanBeNull] TeleportationController teleported);
}