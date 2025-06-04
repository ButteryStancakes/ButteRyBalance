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
                    // don't override Clay Surgeon Overhaul infestations
                    InfestationOverrides.CustomInfestation(__instance.currentLevel.Enemies[__instance.enemyRushIndex].enemyType.name == "ClaySurgeon" ? __instance.enemyRushIndex : -1);
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
                        if (RoundManager.Instance.currentDungeonType == 4 && InfestationOverrides.GetInfesterName() != "ClaySurgeon")
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
                                    Plugin.Logger.LogWarning("Can't increase Barber spawn group because Vent Spawn Fix is not loaded. Please install it and make sure it works correctly");
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
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
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

        [HarmonyPatch(nameof(RoundManager.SpawnOutsideHazards))]
        [HarmonyPrefix]
        static void RoundManager_Pre_SpawnOutsideHazards(RoundManager __instance)
        {
            // don't alter custom levels
            if (__instance.currentLevel.levelID >= Common.NUM_LEVELS)
                return;

            if (RoundManager.Instance?.mapPropsContainer == null)
            {
                Plugin.Logger.LogWarning("Can't create SpawnDenialPoint(s) because mapPropsContainer is missing, this *might* cause desync with other clients");
                return;
            }

            foreach (EntranceTeleport entranceTeleport in Object.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None))
            {
                if (entranceTeleport.isEntranceToBuilding)
                {
                    CreateSpawnDenialPoint(entranceTeleport.entrancePoint.position);
                    Plugin.Logger.LogDebug($"{__instance.currentLevel.name}: SpawnDenialPoint (Entrance)");
                }
            }

            foreach (InteractTrigger interactTrigger in Object.FindObjectsByType<InteractTrigger>(FindObjectsSortMode.None))
            {
                if (interactTrigger.isLadder && interactTrigger.bottomOfLadderPosition != null)
                {
                    CreateSpawnDenialPoint(interactTrigger.bottomOfLadderPosition.position);
                    Plugin.Logger.LogDebug($"{__instance.currentLevel.name}: SpawnDenialPoint (Ladder)");
                }
            }

            // prevent stairs next to ship being blocked
            /*if (__instance.currentLevel.sceneName == "Level9Titan")
                CreateSpawnDenialPoint(new(-15.5352612f, -3.74099994f, 6.6187892f));*/
        }

        static void CreateSpawnDenialPoint(Vector3 pos)
        {
            Transform spawnDenialPoint = new GameObject("SpawnDenialPointBRB")
            {
                tag = "SpawnDenialPoint"
            }.transform;
            spawnDenialPoint.SetParent(RoundManager.Instance.mapPropsContainer.transform);
            spawnDenialPoint.position = pos;
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
        // also run when enemies are first allowed to start spawning
        [HarmonyPatch(nameof(RoundManager.BeginEnemySpawning))]
        [HarmonyPrefix]
        static void RoundManager_Pre_BeginEnemySpawning(RoundManager __instance)
        {
            if (__instance.IsServer)
                InfestationOverrides.SpawnInfestationWave();
        }

        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPrefix]
        static void RoundManager_Pre_SpawnScrapInLevel(RoundManager __instance)
        {
            if (__instance.currentLevel.name == "AdamanceLevel" && Configuration.adamanceBuffScrap.Value)
            {
                if (__instance.currentDungeonType != 4)
                {
                    __instance.currentLevel.minScrap = 19;
                    __instance.currentLevel.maxScrap = 24;
                }
                else
                {
                    // vanilla values, because mineshaft is a bit *too* good...
                    __instance.currentLevel.minScrap = 16;
                    __instance.currentLevel.maxScrap = 19;
                }
            }
        }
    }
}
