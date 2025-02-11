using ButteRyBalance.Overrides.Moons;
using System.Collections.Generic;
using System.Linq;

namespace ButteRyBalance.Overrides
{
    internal class InfestationOverrides
    {
        internal static int infestationEnemyIndex = -1;
        static EnemyType infestationEnemy;
        static float rollbackPowerLevel;

        static List<string> compatibleEnemies = new()
        {
            "HoarderBug",
            "Nutcracker",
            "Butler",
            "MaskedPlayerEnemy",
        };

        static readonly Dictionary<string, Dictionary<string, int>> vanillaLevels = new()
        {
            { "ExperimentationLevel",   ExperimentationOverrides.infestations },
            { "AssuranceLevel",               AssuranceOverrides.infestations },
            { "VowLevel",                           VowOverrides.infestations },
            { "OffenseLevel",                   OffenseOverrides.infestations },
            { "MarchLevel",                       MarchOverrides.infestations },
            { "AdamanceLevel",                 AdamanceOverrides.infestations },
            { "RendLevel",                         RendOverrides.infestations },
            { "DineLevel",                         DineOverrides.infestations },
            { "TitanLevel",                       TitanOverrides.infestations },
            { "ArtificeLevel",                 ArtificeOverrides.infestations },
            { "EmbrionLevel",                   EmbrionOverrides.infestations }
        };

        static List<IntWithRarity> infestationEnemies = [];

        internal static void FilterInfestations()
        {
            if (Configuration.infestationButlers.Value)
            {
                if (!compatibleEnemies.Contains("Butler"))
                    compatibleEnemies.Add("Butler");
            }
            else
                compatibleEnemies.Remove("Butler");

            if (Configuration.infestationMasked.Value)
            {
                if (!compatibleEnemies.Contains("MaskedPlayerEnemy"))
                    compatibleEnemies.Add("MaskedPlayerEnemy");
            }
            else
                compatibleEnemies.Remove("MaskedPlayerEnemy");
        }

        internal static void CustomInfestation()
        {
            SelectableLevel level = RoundManager.Instance.currentLevel;

            RoundManager.Instance.enemyRushIndex = -1;
            RoundManager.Instance.currentMaxInsidePower = RoundManager.Instance.currentLevel.maxEnemyPowerCount;

            infestationEnemies.Clear();
            vanillaLevels.TryGetValue(level.name, out Dictionary<string, int> availableEnemies);

            for (int i = 0; i < level.Enemies.Count; i++)
            {
                if (!compatibleEnemies.Contains(level.Enemies[i].enemyType.name) || level.Enemies[i].rarity == 0)
                    continue;

                if (availableEnemies != null)
                {
                    if (availableEnemies.TryGetValue(level.Enemies[i].enemyType.name, out int weight))
                    {
                        infestationEnemies.Add(new()
                        {
                            id = i,
                            rarity = weight
                        });
                    }
                }
                else
                {
                    infestationEnemies.Add(new()
                    {
                        id = i,
                        rarity = level.Enemies[i].rarity
                    });
                }
            }

            Plugin.Logger.LogDebug($"Infestation weights for \"{level.name}\":");
            foreach (IntWithRarity intWithRarity in infestationEnemies)
                Plugin.Logger.LogDebug($"{level.Enemies[intWithRarity.id].enemyType.name} - {intWithRarity.rarity}");

            infestationEnemyIndex = infestationEnemies[RoundManager.Instance.GetRandomWeightedIndex(infestationEnemies.Select(x => x.rarity).ToArray(), new System.Random(StartOfRound.Instance.randomMapSeed + 5781))].id;
            infestationEnemy = level.Enemies[infestationEnemyIndex].enemyType;

            Plugin.Logger.LogDebug($"Starting {infestationEnemy.name} infestation");
            infestationEnemy.PowerLevel = 0f;
            RoundManager.Instance.increasedInsideEnemySpawnRateIndex = infestationEnemyIndex;
        }

        internal static void EndInfestation()
        {
            if (infestationEnemyIndex < 0)
                return;

            RoundManager.Instance.increasedInsideEnemySpawnRateIndex = -1;
            if (rollbackPowerLevel > 0f)
            {
                rollbackPowerLevel = -1f;
                infestationEnemy.PowerLevel = rollbackPowerLevel;
            }
            infestationEnemyIndex = -1;
            infestationEnemy = null;
        }

        internal static void SpawnInfestationWave()
        {
            if (infestationEnemyIndex < 0)
                return;

            RoundManager.Instance.currentEnemySpawnIndex = 0;
            if (RoundManager.Instance.firstTimeSpawningEnemies)
            {
                foreach (SpawnableEnemyWithRarity enemyWithRarity in RoundManager.Instance.currentLevel.Enemies)
                    enemyWithRarity.enemyType.numberSpawned = 0;

                RoundManager.Instance.firstTimeSpawningEnemies = false;
            }

            List<EnemyVent> availableVents = RoundManager.Instance.allEnemyVents.Where(enemyVent => !enemyVent.occupied).ToList();

            for (int i = 0; i < 2; i++)
            {
                if (infestationEnemy.numberSpawned >= infestationEnemy.MaxCount || availableVents.Count < 1)
                {
                    Plugin.Logger.LogDebug("Can no longer spawn infestation enemies");
                    break;
                }

                float minTime = TimeOfDay.Instance.lengthOfHours * RoundManager.Instance.currentHour;

                int time = (int)(RoundManager.Instance.AnomalyRandom.Next(10, (int)(TimeOfDay.Instance.lengthOfHours * RoundManager.Instance.hourTimeBetweenEnemySpawnBatches)) + minTime);
                EnemyVent vent = availableVents[RoundManager.Instance.AnomalyRandom.Next(availableVents.Count)];

                vent.enemyType = infestationEnemy;
                vent.enemyTypeIndex = infestationEnemyIndex;
                vent.occupied = true;
                vent.spawnTime = time;
                if (TimeOfDay.Instance.hour - RoundManager.Instance.currentHour <= 0)
                    vent.SyncVentSpawnTimeClientRpc(time, infestationEnemyIndex);
                infestationEnemy.numberSpawned++;
                //RoundManager.Instance.enemySpawnTimes.Add(time);
                Plugin.Logger.LogDebug("Added infestation enemy to vent");
            }

            //RoundManager.Instance.enemySpawnTimes.Sort();
        }
    }
}
