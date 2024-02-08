using System;

namespace TurboTowers.Managers.Models
{
    [Serializable]
    public class LevelConfig
    {
        public int LevelNumber { get; }
        public int TurretCount { get; }
        public int TurretHealth { get; }
        public int TurretDamage { get; }
        public int TurretSpeed { get; }
        public int TurretFireRate { get; }
        public int TurretRange { get; }
        public int TurretCost { get; }
        public int TurretSellValue { get; }
        public int TurretUpgradeCost { get; }
        public int TurretUpgradeSellValue { get; }
        public int TurretUpgradeHealth { get; }
        public int TurretUpgradeDamage { get; }
        public int TurretUpgradeSpeed { get; }
        public int TurretUpgradeFireRate { get; }
        public int TurretUpgradeRange { get; }
        public int TurretUpgradeCostMultiplier { get; }
        public int TurretUpgradeSellValueMultiplier { get; }
        public int TurretUpgradeHealthMultiplier { get; }
        public int TurretUpgradeDamageMultiplier { get; }
        public int TurretUpgradeSpeedMultiplier { get; }
        public int TurretUpgradeFireRateMultiplier { get; }
        public int TurretUpgradeRangeMultiplier { get; }
        public int TurretUpgradeCostIncrement { get; }
        public int TurretUpgradeSellValueIncrement { get; }
        public int TurretUpgradeHealthIncrement { get; }
        public int TurretUpgradeDamageIncrement { get; }
        public int TurretUpgradeSpeedIncrement { get; }
        public int TurretUpgradeFireRateIncrement { get; }
        public int TurretUpgradeRangeIncrement { get; }
        public int TurretUpgradeHealthIncrementInterval { get; }
        public int TurretUpgradeDamageIncrementInterval { get; }
        public int TurretUpgradeSpeedIncrementInterval { get; }
        public int TurretUpgradeFireRateIncrementInterval { get; }
        public int TurretUpgradeRangeIncrementInterval { get; }
        public int TurretUpgradeHealthIncrementCount { get; }
        public int TurretUpgradeDamageIncrementCount { get; }
        public int TurretUpgradeSpeedIncrementCount { get; }
        public int TurretUpgradeFireRateIncrementCount { get; }
        public int TurretUpgradeRangeIncrementCount { get; }
        public int TurretUpgradeHealthIncrementCountInterval { get; }
        public int TurretUpgradeDamageIncrementCountInterval { get; }

        public LevelConfig()
        {
            
        }
    }
}