using JetBrains.Annotations;
using UnityEngine;

public interface IActionable
{
        public void PerformAction([CanBeNull] Teleport teleported);
}