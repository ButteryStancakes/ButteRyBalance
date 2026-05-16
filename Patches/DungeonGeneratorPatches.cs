using ButteRyBalance.Network;
using ButteRyBalance.Utilities;
using DunGen;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(DungeonGenerator))]
    static class DungeonGeneratorPatches
    {
        [HarmonyPatch(nameof(DungeonGenerator.ProcessGlobalProps))]
        [HarmonyPostfix]
        static void DungeonGenerator_Post_ProcessGlobalProps(DungeonGenerator __instance)
        {
            if (BRBNetworker.Instance.ProportionalFireExits.Value && Common.fireExitCount > 0)
            {
                GlobalProp[] allFireExits = __instance.Root?.GetComponentsInChildren<GlobalProp>(true)?.Where(globalProp => globalProp.PropGroupID == 1231).ToArray();
                if (allFireExits != null)
                {
                    Plugin.Logger.LogDebug($"Total number of fire exit props: {allFireExits.Length}");
                    if (allFireExits.Length < 1)
                    {
                        foreach (GlobalProp globalProp in __instance.Root.GetComponentsInChildren<GlobalProp>(true))
                        {
                            Plugin.Logger.LogDebug($"Global prop: {globalProp.name} (ID #{globalProp.PropGroupID})");
                        }
                        return;
                    }

                    if (Common.fireExitCount >= allFireExits.Length)
                    {
                        foreach (GlobalProp fireExitProp in allFireExits)
                            fireExitProp.gameObject.SetActive(true);

                        Plugin.Logger.LogWarning($"Enabled all fire exits ({Common.fireExitCount} required)");
                        return;
                    }

                    System.Random fireExitRandom = new System.Random(__instance.Seed);

                    int chunkSize = allFireExits.Length / (Common.fireExitCount + 1);
                    Plugin.Logger.LogDebug($"Fire exit preferred chunk size is {chunkSize}");

                    if (chunkSize < 1)
                    {
                        Plugin.Logger.LogWarning("Fire exit chunk size is 0. Falling back to random spawns");
                        InteriorObjectSpawner.RandomlyActivateFireExits(allFireExits, fireExitRandom);
                        return;
                    }

                    Vector3 mainEntrancePos = Vector3.zero;
                    foreach (SpawnSyncedObject spawnSyncedObject in __instance.Root.GetComponentsInChildren<SpawnSyncedObject>())
                    {
                        if (spawnSyncedObject.spawnPrefab == null)
                            continue;

                        if ((spawnSyncedObject.spawnPrefab.TryGetComponent(out EntranceTeleport entranceTeleport) && !entranceTeleport.isEntranceToBuilding && entranceTeleport.entranceId == 0) || spawnSyncedObject.spawnPrefab.name == "EntranceTeleportA")
                        {
                            mainEntrancePos = spawnSyncedObject.transform.position;
                            break;
                        }
                    }
                    if (mainEntrancePos == Vector3.zero && __instance?.CurrentDungeon?.AllTiles != null && __instance.CurrentDungeon.AllTiles.Count > 0)
                        mainEntrancePos = __instance.CurrentDungeon.AllTiles[0].Bounds.center;

                    // sort by distance to main
                    if (mainEntrancePos != Vector3.zero)
                        allFireExits = allFireExits.OrderBy(fireExit => Vector3.Distance(fireExit.transform.position, mainEntrancePos)).ToArray();
                    else
                    {
                        Plugin.Logger.LogWarning("Could not sort fire exits from main entrance. Falling back to random spawns");
                        InteriorObjectSpawner.RandomlyActivateFireExits(allFireExits, fireExitRandom);
                        return;
                    }

                    List<GlobalProp>[] fireExitChunks = new List<GlobalProp>[Common.fireExitCount + 1];
                    for (int i = 0; i < fireExitChunks.Length; i++)
                        fireExitChunks[i] = [];

                    for (int i = 0; i < allFireExits.Length; i++)
                    {
                        int chunk = Mathf.Min(i / chunkSize, fireExitChunks.Length - 1);
                        //Plugin.Logger.LogDebug($"Fire exit #{i} added to chunk #{chunk}");
                        fireExitChunks[chunk].Add(allFireExits[i]);
                    }

                    for (int i = 1; i < fireExitChunks.Length; i++)
                    {
                        if (fireExitChunks[i].Count < 1)
                        {
                            Plugin.Logger.LogWarning($"Fire exit chunk #{i} includes 0 elements. Falling back to random spawns");
                            InteriorObjectSpawner.RandomlyActivateFireExits(allFireExits, fireExitRandom);
                            return;
                        }
                    }

                    for (int i = 1; i < fireExitChunks.Length; i++)
                    {
                        int index = fireExitRandom.Next(fireExitChunks[i].Count);
                        fireExitChunks[i][index].gameObject.SetActive(true);
                        Plugin.Logger.LogDebug($"Fire exit chunk #{i} ({fireExitChunks[i].Count}) activating fire exit #{index}");
                    }
                }
            }
        }
    }
}
