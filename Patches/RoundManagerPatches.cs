using ButteRyBalance.Network;
using ButteRyBalance.Overrides;
using ButteRyBalance.Overrides.Moons;
using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Netcode;
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

            System.Random tempRandom = new(StartOfRound.Instance.randomMapSeed + 5781);
            if (__instance.currentLevel.name == "ExperimentationLevel" && BRBNetworker.Instance.ExperimentationNoEvents.Value && !BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                __instance.enemyRushIndex = -1;
                __instance.currentMaxInsidePower = __instance.currentLevel.maxEnemyPowerCount;
                __instance.indoorFog.gameObject.SetActive(false);
            }
            else
            {
                if (__instance.IsServer && (__instance.enemyRushIndex >= 0 || (__instance.currentLevel.name == "EmbrionLevel" && BRBNetworker.Instance.EmbrionMega.Value && tempRandom.Next(100) <= 8)) && Configuration.infestationRework.Value && Common.INSTALLED_SPAWN_CYCLE_FIXES)
                {
                    // don't override Clay Surgeon Overhaul infestations
                    InfestationOverrides.CustomInfestation((__instance.enemyRushIndex >= 0 && __instance.currentLevel.Enemies[__instance.enemyRushIndex].enemyType.name == "ClaySurgeon") ? __instance.enemyRushIndex : -1);
                }

                if (__instance.indoorFog.gameObject.activeSelf && BRBNetworker.Instance.RandomIndoorFog.Value)
                    __instance.indoorFog.parameters.meanFreePath = tempRandom.Next(5, 11);
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
                                if (Common.INSTALLED_SPAWN_CYCLE_FIXES)
                                {
                                    barber.spawnInGroupsOf = 2;
                                    Plugin.Logger.LogDebug($"Barber - Dynamic Spawn Settings: 2/8 (Interior ID: {RoundManager.Instance.currentDungeonType})");
                                }
                                else
                                    Plugin.Logger.LogWarning("Can't increase Barber spawn group because Spawn Cycle Fixes is not loaded. Please install it and make sure it works correctly");
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
                    case "AdamanceLevel":
                        if (BRBNetworker.Instance.AdamanceInteriors.Value)
                            MoonOverrides.AdjustInteriors(__instance.currentLevel, AdamanceOverrides.adjustedInteriors);
                        break;
                    case "DineLevel":
                        if (BRBNetworker.Instance.DineMineshafts.Value)
                            MoonOverrides.AdjustInteriors(__instance.currentLevel, DineOverrides.adjustedInteriors);
                        break;
                    case "ArtificeLevel":
                        if (BRBNetworker.Instance.ArtificeInteriors.Value)
                            MoonOverrides.AdjustInteriors(__instance.currentLevel, ArtificeOverrides.adjustedInteriors);
                        break;
                    case "EmbrionLevel":
                        if (BRBNetworker.Instance.EmbrionMega.Value)
                            MoonOverrides.AdjustInteriors(__instance.currentLevel, EmbrionOverrides.adjustedInteriors);
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

            if (__instance.IsServer)
            {
                if (Configuration.spikeTrapDistance.Value)
                {
                    SpikeRoofTrap[] spikeRoofTraps = Object.FindObjectsByType<SpikeRoofTrap>(FindObjectsSortMode.None);
                    EntranceTeleport[] entranceTeleports = Object.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None);
                    MineshaftElevatorController mineshaftElevatorController = __instance.currentMineshaftElevator ?? Object.FindAnyObjectByType<MineshaftElevatorController>();
                    foreach (SpikeRoofTrap spikeRoofTrap in spikeRoofTraps)
                    {
                        bool markedForDeletion = false;

                        if (mineshaftElevatorController?.elevatorBottomPoint != null && Vector3.Distance(spikeRoofTrap.spikeTrapAudio.transform.position, mineshaftElevatorController.elevatorBottomPoint.position) < 7f)
                        {
                            Plugin.Logger.LogDebug($"Spike trap #{spikeRoofTrap.GetInstanceID()} will be destroyed (too close to the elevator)");
                            markedForDeletion = true;
                        }

                        if (!markedForDeletion)
                        {
                            foreach (EntranceTeleport entranceTeleport in entranceTeleports)
                            {
                                if (entranceTeleport.isEntranceToBuilding || entranceTeleport.entrancePoint == null)
                                    continue;

                                if (Vector3.Distance(spikeRoofTrap.spikeTrapAudio.transform.position, entranceTeleport.entrancePoint.position) < 4.5f)
                                {
                                    markedForDeletion = true;
                                    Plugin.Logger.LogDebug($"Spike trap #{spikeRoofTrap.GetInstanceID()} will be destroyed (too close to entrance \"{entranceTeleport.name}\")");
                                    break;
                                }
                            }
                        }

                        if (markedForDeletion)
                        {
                            NetworkObject netObj = spikeRoofTrap.GetComponentInParent<NetworkObject>();
                            if (netObj != null && netObj.IsSpawned)
                                netObj.Despawn();
                            else
                                Plugin.Logger.LogWarning("Error occurred while despawning spike trap (could not find network object, or it was not network spawned yet)");
                        }
                    }
                }

                if ((__instance.currentDungeonType == 0 || __instance.currentDungeonType == 2 || __instance.currentDungeonType == 3) && BRBNetworker.Instance.ApparatusPrice.Value)
                {
                    LungProp apparatus = __instance.mapPropsContainer.GetComponentInChildren<LungProp>();
                    if (apparatus != null && apparatus.isLungDocked && apparatus.scrapValue == 80)
                        BRBNetworker.Instance.SyncScrapPriceRpc(apparatus.NetworkObject, new System.Random(StartOfRound.Instance.randomMapSeed).Next(40, 131), false);
                }
            }

            Common.CacheCaveTiles();
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

            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                int snowmen = __instance.currentLevel.name switch
                {
                    "RendLevel" => BRBNetworker.Instance.RendSnowmen.Value,
                    "DineLevel" => BRBNetworker.Instance.DineSnowmen.Value,
                    "TitanLevel" => BRBNetworker.Instance.TitanSnowmen.Value,
                    _ => 0
                };

                if (snowmen > 0)
                    MoonOverrides.RestoreSnowmen(__instance.currentLevel, snowmen > 1);
            }
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

        [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
        [HarmonyPrefix]
        static void RoundManager_Pre_PlotOutEnemiesForNextHour(RoundManager __instance, ref bool __state)
        {
            __state = false;
            if (__instance.IsServer)
            {
                InfestationOverrides.SpawnInfestationWave();
                if (__instance.currentLevel.name == "AdamanceLevel" && Configuration.adamanceReduceCadavers.Value && __instance.minEnemiesToSpawn < 1 && __instance.timeScript.hour > __instance.hourTimeBetweenEnemySpawnBatches && new System.Random(__instance.playersManager.randomMapSeed / Mathf.RoundToInt(__instance.timeScript.hour * __instance.timeScript.lengthOfHours)).NextDouble() < 0.8)
                {
                    __state = true;
                    __instance.minEnemiesToSpawn = 1;
                    Plugin.Logger.LogDebug($"Adamance: Force inside spawn this batch ({HUDManager.Instance.GetClockTimeFormatted(__instance.timeScript.normalizedTimeOfDay, __instance.timeScript.numberOfHours, false)})");
                }
            }
        }

        [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
        [HarmonyPostfix]
        static void RoundManager_Post_PlotOutEnemiesForNextHour(RoundManager __instance, bool __state)
        {
            if (__state)
                __instance.minEnemiesToSpawn = 0;
        }

        [HarmonyPatch(nameof(RoundManager.SpawnEnemiesOutside))]
        [HarmonyPostfix]
        static void RoundManager_Post_SpawnEnemiesOutside(RoundManager __instance)
        {
            // sometimes, during an infestation, cannotSpawnMoreInsideEnemies can be ignored
            if (__instance.IsServer && __instance.allEnemyVents.Length > 0 && __instance.cannotSpawnMoreInsideEnemies)
                InfestationOverrides.SpawnInfestationWave();
        }

        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPrefix]
        static void RoundManager_Pre_SpawnScrapInLevel(RoundManager __instance, ref float[] __state)
        {
            __state = [1f, 1f];
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                switch (__instance.currentLevel.name)
                {
                    case "AssuranceLevel":
                        if (Configuration.assuranceNerfScrap.Value)
                        {
                            if (__instance.currentDungeonType != 4)
                            {
                                __instance.currentLevel.minScrap = 13;
                                __instance.currentLevel.maxScrap = 16;
                            }
                            else
                            {
                                __instance.currentLevel.minScrap = 11;
                                __instance.currentLevel.maxScrap = 16;
                            }
                        }
                        break;
                    case "VowLevel":
                        if (BRBNetworker.Instance.VowMineshafts.Value)
                        {
                            if (__instance.currentDungeonType != 4)
                            {
                                __instance.currentLevel.minScrap = 12;
                                __instance.currentLevel.maxScrap = 15;
                            }
                            else
                            {
                                // pre-v50 values, because mineshaft is a bit *too* good...
                                __instance.currentLevel.minScrap = 10;
                                __instance.currentLevel.maxScrap = 13;
                            }
                        }
                        break;
                    case "AdamanceLevel":
                        if (Configuration.adamanceBuffScrap.Value)
                        {
                            if (__instance.currentDungeonType != 4 || !BRBNetworker.Instance.AdamanceInteriors.Value)
                            {
                                // v73
                                __instance.currentLevel.minScrap = 16;
                                __instance.currentLevel.maxScrap = 19;
                            }
                            else
                            {
                                // vanilla
                                __instance.currentLevel.minScrap = 14;
                                __instance.currentLevel.maxScrap = 17;
                            }
                        }
                        break;
                    case "DineLevel":
                        if (Configuration.dineScrapPool.Value != Configuration.DineScrap.Rollback)
                        {
                            if (__instance.currentLevel.minScrap >= 200)
                            {
                                if (__instance.currentDungeonType != 4 || !BRBNetworker.Instance.DineMineshafts.Value)
                                {
                                    // vanilla
                                    __instance.currentLevel.minScrap = 200;
                                    __instance.currentLevel.maxScrap = 250;
                                }
                                else
                                {
                                    __instance.currentLevel.minScrap = 220;
                                    __instance.currentLevel.maxScrap = 270;
                                }
                            }
                            if (Configuration.dineScrapPool.Value == Configuration.DineScrap.Consolidate)
                            {
                                __state[0] *= DineOverrides.CONSOLIDATE_AMOUNT;
                                __instance.scrapAmountMultiplier *= __state[0];
                                __state[1] *= DineOverrides.CONSOLIDATE_VALUE;
                                __instance.scrapValueMultiplier *= __state[1];
                            }
                        }
                        break;
                    case "TitanLevel":
                        if (Configuration.titanBuffScrap.Value)
                        {
                            if (__instance.currentDungeonType != 4)
                            {
                                // v50
                                __instance.currentLevel.minScrap = 28;
                                __instance.currentLevel.maxScrap = 36;
                            }
                            else
                            {
                                // vanilla
                                __instance.currentLevel.minScrap = 28;
                                __instance.currentLevel.maxScrap = 32;
                            }
                        }
                        break;
                    case "ArtificeLevel":
                        if (Configuration.artificeBuffScrap.Value)
                        {
                            if (__instance.currentDungeonType != 4)
                            {
                                // v56
                                __instance.currentLevel.minScrap = 31;
                                __instance.currentLevel.maxScrap = 38;
                            }
                            else
                            {
                                // vanilla
                                __instance.currentLevel.minScrap = 26;
                                __instance.currentLevel.maxScrap = 31;
                            }
                        }
                        break;
                }
            }

            // https://discord.com/channels/750645598293590077/1501504390148653066/1503528764108181574
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
                __instance.AnomalyRandom.NextDouble();
        }

        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPostfix]
        static void RoundManager_Post_SpawnScrapInLevel(RoundManager __instance, float[] __state)
        {
            BRBNetworker.Instance.SetScanValueMultiplierRpc(__instance.scrapValueMultiplier);
            __instance.scrapAmountMultiplier /= __state[0];
            __instance.scrapValueMultiplier /= __state[1];
        }

        [HarmonyPatch(nameof(RoundManager.SetLockedDoors))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RoundManager_Trans_SetLockedDoors(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 5; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Div && codes[i - 1].opcode == OpCodes.Ldc_I4_3 && codes[i - 3].opcode == OpCodes.Ldlen)
                {
                    codes.InsertRange(i + 2, [
                        new(OpCodes.Ldloca_S, codes[i - 4].operand),
                        new(OpCodes.Ldloca_S, codes[i + 1].operand),
                        new(OpCodes.Call, AccessTools.Method(typeof(InteriorObjectSpawner), nameof(InteriorObjectSpawner.PostProcessKeyNodes)))
                    ]);
                    Plugin.Logger.LogDebug($"Transpiler (Locked doors): Post process key spawns");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning($"Locked doors transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
        [HarmonyPostfix]
        static void RoundManager_Post_DespawnPropsAtEndOfRound(RoundManager __instance)
        {
            if (__instance.IsServer)
            {
                if (Configuration.cruiserItemSafety.Value && __instance.playersManager?.attachedVehicle != null && __instance.playersManager.attachedVehicle.vehicleID == 0 && !Common.INSTALLED_VERSION55_COMPANY_CRUISER)
                {
                    Bounds onTopOfTruckBounds = __instance.playersManager.attachedVehicle.ontopOfTruckCollider.bounds;

                    GrabbableObject[] cruiserItems = __instance.playersManager.attachedVehicle?.GetComponentsInChildren<GrabbableObject>();
                    foreach (GrabbableObject cruiserItem in cruiserItems)
                    {
                        if (__instance.playersManager.attachedVehicle.backDoorOpen || onTopOfTruckBounds.ClosestPoint(cruiserItem.transform.position) == cruiserItem.transform.position)
                        {
                            Plugin.Logger.LogDebug($"Cruiser item \"{cruiserItem.name}\" (#{cruiserItem.GetInstanceID()}) lost in orbit");
                            if (cruiserItem.NetworkObject != null && cruiserItem.NetworkObject.IsSpawned)
                                cruiserItem.NetworkObject.Despawn();
                            else
                                Object.Destroy(cruiserItem.gameObject);
                        }
                    }
                }

                Common.CleanTemporaryNetworkObjects();
            }
        }

        [HarmonyPatch(nameof(RoundManager.DestroyTreeAtPosition))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RoundManager_Trans_DestroyTreeAtPosition(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            FieldInfo breakTreeAudio2 = AccessTools.Field(typeof(RoundManager), nameof(RoundManager.breakTreeAudio2));
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && (FieldInfo)codes[i].operand == breakTreeAudio2)
                {
                    codes.Insert(i + 2, new(OpCodes.Call, AccessTools.Method(typeof(VehicleControllerPatches), nameof(VehicleControllerPatches.JustDestroyedTree))));
                    Plugin.Logger.LogDebug($"Transpiler (Destroy tree): Send signal to Cruiser");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning($"Destroy tree transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(RoundManager.SpawnSyncedProps))]
        [HarmonyPostfix]
        static void RoundManager_Post_SpawnSyncedProps(RoundManager __instance)
        {
            if (!__instance.IsServer || BRBNetworker.Instance.MoonsKillSwitch.Value)
                return;

            bool resyncFireExits = false;
            switch (Common.lastSceneLoaded)
            {
                case "Level4March":
                    resyncFireExits = true;
                    break;

                case "Level6Dine":
                    resyncFireExits = BRBNetworker.Instance.DineFireExits.Value;
                    break;

                case "Level7Offense":
                    resyncFireExits = BRBNetworker.Instance.OffenseFireExits.Value;
                    break;
            }

            if (resyncFireExits)
            {
                if (SceneOverrides.entranceTeleport1?.NetworkObject != null)
                {
                    BRBNetworker.Instance.SyncFireExitRpc(SceneOverrides.entranceTeleport1.NetworkObject, 1, fresh: false);
                    if (SceneOverrides.entranceTeleport2?.NetworkObject != null)
                    {
                        BRBNetworker.Instance.SyncFireExitRpc(SceneOverrides.entranceTeleport2.NetworkObject, 2, fresh: false);

                        // *supposed* to be null on Offense
                        if (SceneOverrides.entranceTeleport3?.NetworkObject != null)
                            BRBNetworker.Instance.SyncFireExitRpc(SceneOverrides.entranceTeleport3.NetworkObject, 3, fresh: false);

                        return;
                    }
                }
                Plugin.Logger.LogWarning($"Failed to resync fire exits as host:\n#1 - {SceneOverrides.entranceTeleport1}\n#2 - {SceneOverrides.entranceTeleport2}\n#3 - {SceneOverrides.entranceTeleport3}");
            }
        }

        [HarmonyPatch(nameof(RoundManager.SetExitIDs))]
        [HarmonyFinalizer]
        [HarmonyPriority(Priority.Last)]
        static void RoundManager_Post_SetExitIDs()
        {
            if (SceneOverrides.entranceTeleport1 != null || SceneOverrides.entranceTeleport2 != null || SceneOverrides.entranceTeleport3 != null)
                Plugin.Logger.LogDebug("Running final synchronization step for fire exits");

            if (SceneOverrides.entranceTeleport1 != null)
            {
                if (SceneOverrides.entranceTeleport1.entranceId != 1)
                {
                    Plugin.Logger.LogWarning($"Fire exit \"{SceneOverrides.entranceTeleport1.name}\" is expressing an incorrect ID of {SceneOverrides.entranceTeleport1.entranceId} on this client (should be 1)");
                    SceneOverrides.entranceTeleport1.entranceId = 1;
                }
                else
                    Plugin.Logger.LogDebug($"Fire exit \"{SceneOverrides.entranceTeleport1.name}\" is nutritious and healthy");
            }

            if (SceneOverrides.entranceTeleport2 != null)
            {
                if (SceneOverrides.entranceTeleport2.entranceId != 2)
                {
                    Plugin.Logger.LogWarning($"Fire exit \"{SceneOverrides.entranceTeleport2.name}\" is expressing an incorrect ID of {SceneOverrides.entranceTeleport2.entranceId} on this client (should be 2)");
                    SceneOverrides.entranceTeleport2.entranceId = 2;
                }
                else
                    Plugin.Logger.LogDebug($"Fire exit \"{SceneOverrides.entranceTeleport2.name}\" is nutritious and healthy");
            }

            if (SceneOverrides.entranceTeleport3 != null)
            {
                if (SceneOverrides.entranceTeleport3.entranceId != 3)
                {
                    Plugin.Logger.LogWarning($"Fire exit \"{SceneOverrides.entranceTeleport3.name}\" is expressing an incorrect ID of {SceneOverrides.entranceTeleport3.entranceId} on this client (should be 3)");
                    SceneOverrides.entranceTeleport3.entranceId = 3;
                }
                else
                    Plugin.Logger.LogDebug($"Fire exit \"{SceneOverrides.entranceTeleport3.name}\" is nutritious and healthy");
            }
        }
    }
}
