using ButteRyBalance.Overrides.Moons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ButteRyBalance.Overrides
{
    internal class InfestationOverrides
    {
        static int infestationEnemyIndex = -1;
        static EnemyType infestationEnemy;
        static float rollbackPowerLevel = -1f;
        static int rollbackMaxCount = -1;
        static int hourOfLastInfestationWave = -1;

        static List<string> compatibleEnemies = new()
        {
            "HoarderBug",
            "Nutcracker",
            "Butler",
            "MaskedPlayerEnemy",
            "ClaySurgeon",
            "Crawler",
            "SpringMan",
            "Stingray"
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

            if (Configuration.infestationBarbers.Value && RoundManager.Instance.currentDungeonType != 4)
            {
                if (!compatibleEnemies.Contains("ClaySurgeon"))
                    compatibleEnemies.Add("ClaySurgeon");
            }
            else
                compatibleEnemies.Remove("ClaySurgeon");

            if (Configuration.infestationThumpers.Value)
            {
                if (!compatibleEnemies.Contains("Crawler"))
                    compatibleEnemies.Add("Crawler");
            }
            else
                compatibleEnemies.Remove("Crawler");

            if (Configuration.infestationSnareFlea.Value)
            {
                if (!compatibleEnemies.Contains("Centipede"))
                    compatibleEnemies.Add("Centipede");
            }
            else
                compatibleEnemies.Remove("Centipede");

            if (Configuration.infestationCoilhead.Value)
            {
                if (!compatibleEnemies.Contains("SpringMan"))
                    compatibleEnemies.Add("SpringMan");
            }
            else
                compatibleEnemies.Remove("SpringMan");

            if (Configuration.infestationGunkfish.Value)
            {
                if (!compatibleEnemies.Contains("Stingray"))
                    compatibleEnemies.Add("Stingray");
            }
            else
                compatibleEnemies.Remove("Stingray");
        }

        internal static void CustomInfestation(int forceIndex = -1)
        {
            SelectableLevel level = RoundManager.Instance.currentLevel;

            infestationEnemyIndex = RoundManager.Instance.enemyRushIndex; // allow defaulting to vanilla selection, if no additional types enabled
            RoundManager.Instance.enemyRushIndex = -1;

            infestationEnemies.Clear();
            vanillaLevels.TryGetValue(level.name, out Dictionary<string, int> availableEnemies);

            System.Random rand = new(StartOfRound.Instance.randomMapSeed + 5781);

            if (forceIndex >= 0)
                infestationEnemyIndex = forceIndex;
            else if (compatibleEnemies.Except(["HoardingBug", "Nutcracker"]).Any()) // default otherwise
            {
                FilterInfestations();
                for (int i = 0; i < level.Enemies.Count; i++)
                {
                    if (!compatibleEnemies.Contains(level.Enemies[i].enemyType.name) || level.Enemies[i].rarity == 0)
                        continue;

                    if (availableEnemies != null)
                    {
                        if (availableEnemies.TryGetValue(level.Enemies[i].enemyType.name, out int weight))
                            infestationEnemies.Add(new(i, weight, null));
                    }
                    else
                        infestationEnemies.Add(new(i, level.Enemies[i].rarity, null));
                }

                Plugin.Logger.LogDebug($"Infestation weights for \"{level.name}\":");
                foreach (IntWithRarity intWithRarity in infestationEnemies)
                    Plugin.Logger.LogDebug($"{level.Enemies[intWithRarity.id].enemyType.name} - {intWithRarity.rarity}");

                infestationEnemyIndex = infestationEnemies[RoundManager.Instance.GetRandomWeightedIndex(infestationEnemies.Select(x => x.rarity).ToArray(), rand)].id;
            }

            infestationEnemy = level.Enemies[infestationEnemyIndex].enemyType;

            Plugin.Logger.LogDebug($"Starting {infestationEnemy.name} infestation");
            rollbackPowerLevel = infestationEnemy.PowerLevel;
            infestationEnemy.PowerLevel = 0f;
            switch (infestationEnemy.name)
            {
                case "HoardingBug":
                    rollbackMaxCount = infestationEnemy.MaxCount;
                    infestationEnemy.MaxCount = 8;
                    break;
                case "Crawler":
                    rollbackMaxCount = infestationEnemy.MaxCount;
                    infestationEnemy.MaxCount = Mathf.Min(infestationEnemy.MaxCount * 2, 8);
                    break;
                case "Centipede":
                    rollbackMaxCount = infestationEnemy.MaxCount;
                    infestationEnemy.MaxCount = 20;
                    break;
            }

            if (infestationEnemy.name != "ClaySurgeon")
            {
                RoundManager.Instance.currentMaxInsidePower = RoundManager.Instance.currentLevel.maxEnemyPowerCount;
                RoundManager.Instance.increasedInsideEnemySpawnRateIndex = infestationEnemyIndex;
            }
            else
                RoundManager.Instance.currentMaxInsidePower = 0f;
        }

        internal static void EndInfestation()
        {
            hourOfLastInfestationWave = -1;

            if (infestationEnemyIndex < 0)
                return;

            RoundManager.Instance.increasedInsideEnemySpawnRateIndex = -1;
            if (rollbackPowerLevel > 0f)
            {
                infestationEnemy.PowerLevel = rollbackPowerLevel;
                rollbackPowerLevel = -1f;
            }
            if (rollbackMaxCount > 0)
            {
                infestationEnemy.MaxCount = rollbackMaxCount;
                rollbackMaxCount = -1;
            }
            infestationEnemyIndex = -1;
            infestationEnemy = null;
        }

        internal static void SpawnInfestationWave()
        {
            if (infestationEnemyIndex < 0 || RoundManager.Instance.currentHour == hourOfLastInfestationWave)
                return;

            hourOfLastInfestationWave = RoundManager.Instance.currentHour;

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

                int time = RoundManager.Instance.AnomalyRandom.Next((int)(TimeOfDay.Instance.lengthOfHours * TimeOfDay.Instance.hour) + 10, (int)(TimeOfDay.Instance.lengthOfHours * (RoundManager.Instance.currentHour + 1)));
                EnemyVent vent = availableVents[RoundManager.Instance.AnomalyRandom.Next(availableVents.Count)];

                vent.enemyType = infestationEnemy;
                vent.enemyTypeIndex = infestationEnemyIndex;
                vent.occupied = true;
                vent.spawnTime = time;
                if (TimeOfDay.Instance.hour - RoundManager.Instance.currentHour <= 0)
                    vent.SyncVentSpawnTimeClientRpc(time, infestationEnemyIndex);
                infestationEnemy.numberSpawned++;
                Plugin.Logger.LogDebug("Added infestation enemy to vent");

                // reduce scaling of barber infestation
                if (infestationEnemy.name == "ClaySurgeon")
                    break;

                availableVents.Remove(vent);
            }
        }

        internal static string GetInfesterName()
        {
            if (infestationEnemyIndex < 0)
                return string.Empty;

            return RoundManager.Instance.currentLevel.Enemies[infestationEnemyIndex].enemyType.name;
        }
    }
}
