using System.Collections.Generic;
using System.Linq;

namespace ButteRyBalance.Overrides.Moons
{
    internal class MoonOverrides
    {
        internal static int minScrap = -1, maxScrap = -1, adjustedEclipse = -1, powerCount = -1, outsidePowerCount = -1;
        internal static Dictionary<string, int> adjustedScrap = [], adjustedEnemies = [];

        internal static void Apply(SelectableLevel level)
        {
            AdjustScrap(level);
            AdjustEnemies(level);

            if (adjustedEclipse >= 0)
            {
                RandomWeatherWithVariables eclipse = level.randomWeathers.FirstOrDefault(randomWeather => randomWeather.weatherType == LevelWeatherType.Eclipsed);
                if (eclipse != null)
                {
                    Plugin.Logger.LogDebug($"{level.name}.randomWeathers.Eclipsed.weatherVariable: {eclipse.weatherVariable} -> {adjustedEclipse}");
                    eclipse.weatherVariable = adjustedEclipse;
                }
                adjustedEclipse = -1;
            }
        }

        static void AdjustScrap(SelectableLevel level)
        {
            if (minScrap >= 0)
            {
                Plugin.Logger.LogDebug($"{level.name}.minScrap: {level.minScrap} => {minScrap}");
                level.minScrap = minScrap;
                minScrap = -1;
            }
            if (maxScrap >= 0)
            {
                Plugin.Logger.LogDebug($"{level.name}.maxScrap: {level.maxScrap} => {maxScrap}");
                level.maxScrap = maxScrap;
                maxScrap = -1;
            }

            if (adjustedScrap.Count > 0)
            {
                foreach (SpawnableItemWithRarity spawnableItemWithRarity in level.spawnableScrap)
                {
                    if (adjustedScrap.TryGetValue(spawnableItemWithRarity.spawnableItem.name, out int weight))
                    {
                        Plugin.Logger.LogDebug($"{level.name}.spawnableScrap: {spawnableItemWithRarity.spawnableItem.itemName} - {spawnableItemWithRarity.rarity} => {weight}");
                        spawnableItemWithRarity.rarity = weight;
                        adjustedScrap.Remove(spawnableItemWithRarity.spawnableItem.name);
                    }
                }
                if (adjustedScrap.Count > 0)
                {
                    foreach (KeyValuePair<string, int> itemID in adjustedScrap)
                    {
                        Item scrap = StartOfRound.Instance.allItemsList.itemsList.FirstOrDefault(item => item.name == itemID.Key);
                        if (scrap != null)
                        {
                            level.spawnableScrap.Add(new()
                            {
                                spawnableItem = scrap,
                                rarity = itemID.Value
                            });
                        }
                        Plugin.Logger.LogDebug($"{level.name}.spawnableScrap: +({scrap.itemName}, {itemID.Value})");
                    }
                }
                adjustedScrap.Clear();
            }
        }

        static void AdjustEnemies(SelectableLevel level)
        {
            if (powerCount >= 0)
            {
                Plugin.Logger.LogDebug($"{level.name}.maxEnemyPowerCount: {level.maxEnemyPowerCount} => {powerCount}");
                level.maxEnemyPowerCount = powerCount;
                powerCount = -1;
            }
            if (outsidePowerCount >= 0)
            {
                Plugin.Logger.LogDebug($"{level.name}.maxOutsideEnemyPowerCount: {level.maxOutsideEnemyPowerCount} => {outsidePowerCount}");
                level.maxOutsideEnemyPowerCount = outsidePowerCount;
                outsidePowerCount = -1;
            }

            if (adjustedEnemies.Count > 0)
            {
                foreach (KeyValuePair<string, int> adjustedEnemy in adjustedEnemies)
                {
                    if (Common.enemies.TryGetValue(adjustedEnemy.Key, out EnemyType enemy))
                    {
                        List<SpawnableEnemyWithRarity> weightsList = null;
                        if (enemy.isDaytimeEnemy)
                            weightsList = level.DaytimeEnemies;
                        // mimics are tagged wrong
                        else if (enemy.isOutsideEnemy && enemy.name != "MaskedPlayerEnemy")
                            weightsList = level.OutsideEnemies;
                        else
                            weightsList = level.Enemies;

                        SpawnableEnemyWithRarity enemyWeight = weightsList.FirstOrDefault(spawnableEnemyWithRarity => spawnableEnemyWithRarity.enemyType == enemy);
                        if (enemyWeight != null)
                        {
                            Plugin.Logger.LogDebug($"{level.name}: {enemyWeight.enemyType.enemyName} - {enemyWeight.rarity} => {adjustedEnemy.Value}");
                            enemyWeight.rarity = adjustedEnemy.Value;
                        }
                    }
                }
                adjustedEnemies.Clear();
            }
        }

        internal static void AdjustInteriors(SelectableLevel level, Dictionary<int, int> adjustedInteriors)
        {
            foreach (IntWithRarity interior in level.dungeonFlowTypes)
            {
                if (adjustedInteriors.TryGetValue(interior.id, out int weight))
                {
                    Plugin.Logger.LogDebug($"{level.name}.dungeonFlowTypes: {RoundManager.Instance.dungeonFlowTypes[interior.id].dungeonFlow.name}, {interior.rarity} -> {weight}");
                    interior.rarity = weight;
                }
            }
        }
    }
}
