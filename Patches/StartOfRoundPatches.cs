using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    class StartOfRoundPatches
    {
        internal static bool skipWeatherPatch = true;

        [HarmonyPatch(nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        static void StartOfRound_Post_Awake(StartOfRound __instance)
        {
            // cache references to all vanilla enemies
            if (Common.enemies.Count < 1)
            {
                SelectableLevel testAllEnemiesLevel = Object.FindAnyObjectByType<QuickMenuManager>()?.testAllEnemiesLevel;
                if (testAllEnemiesLevel != null)
                {
                    foreach (List<SpawnableEnemyWithRarity> enemyList in new List<SpawnableEnemyWithRarity>[]{
                        testAllEnemiesLevel.Enemies,
                        testAllEnemiesLevel.OutsideEnemies,
                        testAllEnemiesLevel.DaytimeEnemies
                    })
                    {
                        foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in enemyList)
                        {
                            if (!Common.enemies.ContainsKey(spawnableEnemyWithRarity.enemyType.name))
                                Common.enemies.Add(spawnableEnemyWithRarity.enemyType.name, spawnableEnemyWithRarity.enemyType);
                        }
                    }
                }
            }

            // need to add masked enemies to indoors on all clients so vent sounds and infestations will sync
            EnemyType masked = Common.enemies["MaskedPlayerEnemy"];
            if (masked != null)
            {
                foreach (SelectableLevel level in __instance.levels)
                {
                    if ((level.sceneName == "Level2Assurance" || level.sceneName == "Level6Dine" || level.sceneName == "Level7Offense") && !level.Enemies.Any(enemy => enemy.enemyType == masked))
                    {
                        level.Enemies.Add(new()
                        {
                            enemyType = masked,
                            rarity = 0
                        });
                        Plugin.Logger.LogDebug($"Added Masked to {level.name} pool on client (fallback)");
                    }
                }
            }
            else
                Plugin.Logger.LogWarning("Failed to reference Masked enemy type. This should never happen");

            BRBNetworker.Create();
        }

        [HarmonyPatch(nameof(StartOfRound.SetPlanetsWeather))]
        [HarmonyPrefix]
        static void StartOfRound_Pre_SetPlanetsWeather(StartOfRound __instance, ref int connectedPlayersOnServer)
        {
            if (skipWeatherPatch)
            {
                skipWeatherPatch = false;
                return;
            }

            if (BRBNetworker.Instance.MultiplayerWeather.Value)
                connectedPlayersOnServer = __instance.connectedPlayersAmount;

            if (BRBNetworker.Instance.MarchRainy.Value)
            {
                SelectableLevel marchLevel = __instance.levels.FirstOrDefault(level => level.name == "MarchLevel");
                if (marchLevel != null)
                {
                    marchLevel.overrideWeatherType = LevelWeatherType.Rainy;
                    marchLevel.overrideWeather = true;
                    if (!marchLevel.randomWeathers.Any(randomWeather => randomWeather.weatherType == LevelWeatherType.None))
                    {
                        marchLevel.randomWeathers.AddItem(new()
                        {
                            weatherType = LevelWeatherType.None
                        });
                        Plugin.Logger.LogDebug($"Added mild weather to {marchLevel.name} pool");
                    }
                }
            }
        }
    }
}
