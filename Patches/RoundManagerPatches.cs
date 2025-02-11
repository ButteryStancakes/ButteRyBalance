using ButteRyBalance.Network;
using ButteRyBalance.Overrides;
using ButteRyBalance.Overrides.Moons;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    class RoundManagerPatches
    {
        [HarmonyPatch(nameof(RoundManager.RefreshEnemiesList))]
        [HarmonyPostfix]
        static void RoundManager_Post_RefreshEnemiesList(RoundManager __instance)
        {
            InfestationOverrides.EndInfestation();

            if (__instance.currentLevel.name == "ExperimentationLevel" && BRBNetworker.Instance.ExperimentationNoEvents.Value && !BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                __instance.enemyRushIndex = -1;
                __instance.currentMaxInsidePower = __instance.currentLevel.maxEnemyPowerCount;
                __instance.indoorFog.gameObject.SetActive(false);
            }
            else
            {
                if (__instance.IsServer && __instance.enemyRushIndex >= 0 && Configuration.infestationRework.Value)
                {
                    // don't override ClaySurgeonOverhaul infestations
                    if (__instance.currentLevel.Enemies[__instance.enemyRushIndex].enemyType.name != "ClaySurgeon")
                        InfestationOverrides.CustomInfestation();
                }

                if (__instance.indoorFog.gameObject.activeSelf && BRBNetworker.Instance.RandomIndoorFog.Value)
                    __instance.indoorFog.parameters.meanFreePath = new System.Random(StartOfRound.Instance.randomMapSeed + 5781).Next(5, 11);
            }

            if (__instance.IsServer)
            {
                if (Configuration.barberDynamicSpawns.Value)
                {
                    if (Common.enemies.TryGetValue("ClaySurgeon", out EnemyType barber))
                    {
                        if (RoundManager.Instance.currentDungeonType == 4)
                        {
                            barber.MaxCount = 1;
                            barber.spawnInGroupsOf = 1;
                            Plugin.Logger.LogDebug("Barber - Dynamic Spawn Settings: 1/1 (Mineshaft)");
                        }
                        else
                        {
                            if (Common.INSTALLED_BARBER_FIXES)
                            {
                                barber.MaxCount = 8;
                                if (Common.INSTALLED_VENT_SPAWN_FIX)
                                {
                                    barber.spawnInGroupsOf = 2;
                                    Plugin.Logger.LogDebug($"Barber - Dynamic Spawn Settings: 2/8 (Interior ID: {RoundManager.Instance.currentDungeonType})");
                                }
                                else
                                    Plugin.Logger.LogWarning("Can't increase Barber spawn group because Vent Spawn Fixis not loaded. Please install it and make sure it works correctly");
                            }
                            else
                                Plugin.Logger.LogWarning("Can't increase Barber max count because Barber Fixes is not loaded. Please install it and make sure it works correctly, or disable \"Dynamic Spawn Settings\" in the \"Enemy.Barber\" config");
                        }
                    }
                    else
                        Plugin.Logger.LogWarning("Failed to reference Barber enemy type. This should never happen");
                }
            }
        }

        [HarmonyPatch(nameof(RoundManager.GenerateNewFloor))]
        [HarmonyPrefix]
        static void RoundManager_Pre_GenerateNewFloor(RoundManager __instance)
        {
            switch (__instance.currentLevel.name)
            {
                case "VowLevel":
                    if (BRBNetworker.Instance.VowMineshafts.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, VowOverrides.adjustedInteriors);
                    break;
                case "OffenseLevel":
                    if (BRBNetworker.Instance.OffenseMineshafts.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, OffenseOverrides.adjustedInteriors);
                    break;
                case "RendLevel":
                    if (BRBNetworker.Instance.RendMineshafts.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, RendOverrides.adjustedInteriors);
                    break;
                case "TitanLevel":
                    if (BRBNetworker.Instance.TitanMineshafts.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, TitanOverrides.adjustedInteriors);
                    break;
                case "ArtificeLevel":
                    if (BRBNetworker.Instance.ArtificeInteriors.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, ArtificeOverrides.adjustedInteriors);
                    break;
                case "EmbrionLevel":
                    if (BRBNetworker.Instance.EmbrionMineshafts.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, EmbrionOverrides.adjustedInteriors);
                    if (BRBNetworker.Instance.EmbrionMega.Value)
                        MoonOverrides.AdjustInteriors(__instance.currentLevel, EmbrionOverrides.megaInteriors);
                    break;
            }
        }

        [HarmonyPatch(nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
        [HarmonyPostfix]
        static void RoundManager_Post_FinishGeneratingNewLevelClientRpc(RoundManager __instance)
        {
            if (StartOfRound.Instance.currentLevel.name == "ArtificeLevel" && Common.INSTALLED_ARTIFICE_BLIZZARD)
            {
                Common.artificeBlizzard = GameObject.Find("/Systems/Audio/BlizzardAmbience");
                if (Common.artificeBlizzard != null)
                    Plugin.Logger.LogDebug("Artifice Blizzard compatibility success");
            }
        }

        [HarmonyPatch(nameof(RoundManager.SetToCurrentLevelWeather))]
        [HarmonyPostfix]
        static void RoundManager_Post_SetToCurrentLevelWeather(RoundManager __instance)
        {
            if (__instance.currentLevel.name == "DineLevel" && TimeOfDay.Instance.currentLevelWeather == LevelWeatherType.Flooded && BRBNetworker.Instance.DineFloods.Value)
            {
                // use v50 beta values since main entrance was moved down in v60
                TimeOfDay.Instance.currentWeatherVariable = -16f;
                TimeOfDay.Instance.currentWeatherVariable2 = -5f;
            }
        }

        // so it runs after currentHour is incremented, but before cannotSpawnMoreInsideEnemies is set
        [HarmonyPatch(nameof(RoundManager.SpawnEnemiesOutside))]
        [HarmonyPostfix]
        static void RoundManager_Post_SpawnEnemiesOutside(RoundManager __instance)
        {
            if (__instance.IsServer)
                InfestationOverrides.SpawnInfestationWave();
        }
        [HarmonyPatch(nameof(RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies))]
        [HarmonyPostfix]
        static void RoundManager_Post_AdvanceHourAndSpawnNewBatchOfEnemies(RoundManager __instance)
        {
            // infestations will assign to this list before it gets cleared, causing spawns to "lock up"
            if (__instance.IsServer)
            {
                __instance.enemySpawnTimes.Clear();
                foreach (EnemyVent enemyVent in __instance.allEnemyVents)
                {
                    if (enemyVent.occupied)
                        __instance.enemySpawnTimes.Add((int)enemyVent.spawnTime);
                }
                __instance.enemySpawnTimes.Sort();
            }
        }
    }
}
