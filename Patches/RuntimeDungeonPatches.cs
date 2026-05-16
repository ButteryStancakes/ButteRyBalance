using ButteRyBalance.Network;
using DunGen;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using static DunGen.Graph.DungeonFlow;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(RuntimeDungeon))]
    static class RuntimeDungeonPatches
    {
        const float DINE_REND_DIFF = 1.3f / 1.2f;

        [HarmonyPatch(nameof(RuntimeDungeon.Generate))]
        [HarmonyPrefix]
        [HarmonyBefore(Plugin.GUID_LETHAL_LEVEL_LOADER)]
        [HarmonyPriority(Priority.VeryLow)]
        static void RuntimeDungeon_Pre_Generate(RuntimeDungeon __instance)
        {
            int bonusFireExits = 0;
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                switch (RoundManager.Instance.currentLevel.name)
                {
                    case "OffenseLevel":
                        if (BRBNetworker.Instance.OffenseFireExits.Value)
                            bonusFireExits = 1;
                        break;
                    case "RendLevel":
                        if (BRBNetworker.Instance.RendShrink.Value)
                        {
                            // 1.8x -> 1.6x
                            __instance.Generator.LengthMultiplier /= 1.125f;
                            Plugin.Logger.LogDebug("Shrink Rend");
                        }
                        break;
                    case "DineLevel":
                        if (BRBNetworker.Instance.DineFireExits.Value)
                        {
                            // 1.8x -> 1.95x
                            __instance.Generator.LengthMultiplier *= DINE_REND_DIFF;
                            Plugin.Logger.LogDebug("Increase Dine");

                            bonusFireExits = 2;
                        }
                        break;
                    case "ArtificeLevel":
                        if (RoundManager.Instance.currentDungeonType != 4 && BRBNetworker.Instance.ArtificeInteriors.Value)
                        {
                            // 1.8x -> 2x
                            __instance.Generator.LengthMultiplier /= 0.9f;
                            Plugin.Logger.LogDebug("Increase Artifice factory/manor");
                        }
                        break;
                    case "EmbrionLevel":
                        if (BRBNetworker.Instance.EmbrionMega.Value)
                        {
                            switch (RoundManager.Instance.currentDungeonType)
                            {
                                case 0:
                                    __instance.Generator.LengthMultiplier = 2.35f * RoundManager.Instance.mapSizeMultiplier;
                                    break;
                                case 1:
                                    __instance.Generator.LengthMultiplier = 2.4f * RoundManager.Instance.mapSizeMultiplier;
                                    break;
                                case 4:
                                    __instance.Generator.LengthMultiplier = 2.2f * RoundManager.Instance.mapSizeMultiplier;
                                    break;
                                default:
                                    __instance.Generator.LengthMultiplier *= 3f;
                                    break;
                            }
                        }
                        break;
                }
            }

            // just to avoid rounding errors with other clients
            __instance.Generator.LengthMultiplier = Mathf.Floor(__instance.Generator.LengthMultiplier * 100f) / 100f;

            Plugin.Logger.LogDebug($"Final length multiplier: {__instance.Generator.LengthMultiplier} (Interior #{RoundManager.Instance.currentDungeonType}, seed: {StartOfRound.Instance.randomMapSeed})");

            if (BRBNetworker.Instance.ProportionalFireExits.Value || (!BRBNetworker.Instance.MoonsKillSwitch.Value && (BRBNetworker.Instance.OffenseFireExits.Value || BRBNetworker.Instance.DineFireExits.Value)))
            {
                GlobalPropSettings alleyExitDoorContainer = __instance.Generator.DungeonFlow?.GlobalProps?.FirstOrDefault(globalPropSettings => globalPropSettings.ID == 1231);
                if (alleyExitDoorContainer != null)
                {
                    EntranceTeleport[] entranceTeleports = Object.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None);
                    Common.fireExitCount = bonusFireExits;
                    foreach (EntranceTeleport entranceTeleport in entranceTeleports)
                    {
                        if (entranceTeleport.entranceId == 0 || !entranceTeleport.isEntranceToBuilding)
                            continue;

                        if (Common.extraFireExits.Contains(entranceTeleport))
                            continue;

                        Common.fireExitCount++;
                    }

                    if (BRBNetworker.Instance.ProportionalFireExits.Value)
                    {
                        alleyExitDoorContainer.Count.Min = 0;
                        alleyExitDoorContainer.Count.Max = 0;
                    }
                    else
                    {
                        alleyExitDoorContainer.Count.Min = Common.fireExitCount;
                        alleyExitDoorContainer.Count.Max = Common.fireExitCount;
                    }
                    Plugin.Logger.LogDebug($"Fire exits: {Common.fireExitCount}");
                }
                else
                    Plugin.Logger.LogError("Failed to adjust interior's fire exit count");
            }

            Common.extraFireExits.Clear();
        }
    }
}
