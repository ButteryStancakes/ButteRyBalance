using ButteRyBalance.Network;
using ButteRyBalance.Overrides;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    class StartOfRoundPatches
    {
        [HarmonyPatch(nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        static void StartOfRound_Post_Awake()
        {
            BRBNetworker.Create();
            OverrideCoordinator.Init();

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
        }
    }
}
