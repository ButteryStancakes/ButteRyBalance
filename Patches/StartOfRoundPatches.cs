using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    static class StartOfRoundPatches
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

            // need to add enemies to indoors on all clients so vent sounds and infestations will sync
            EnemyType masked = Common.enemies["MaskedPlayerEnemy"], butler = Common.enemies["Butler"];
            if (masked != null && butler != null)
            {
                foreach (SelectableLevel level in __instance.levels)
                {
                    if (masked != null && (level.sceneName == "Level2Assurance" /*|| level.sceneName == "Level6Dine"*/ || level.sceneName == "Level7Offense") && !level.Enemies.Any(enemy => enemy.enemyType == masked))
                    {
                        level.Enemies.Add(new(masked, 0));
                        Plugin.Logger.LogDebug($"Added Masked to {level.name} pool on client (fallback)");
                    }
                    else if (butler != null && level.sceneName == "Level10Adamance" && !level.Enemies.Any(enemy => enemy.enemyType == butler))
                    {
                        level.Enemies.Add(new(butler, 0));
                        Plugin.Logger.LogDebug($"Added butlers to {level.name} pool on client (fallback)");
                    }
                }
            }
            else
                Plugin.Logger.LogWarning("Failed to reference \"masked\" or butler enemy types. This should never happen");

            BRBNetworker.Create();
        }
    }
}
