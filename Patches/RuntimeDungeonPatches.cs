using ButteRyBalance.Network;
using DunGen;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(RuntimeDungeon))]
    static class RuntimeDungeonPatches
    {
        [HarmonyPatch(nameof(RuntimeDungeon.Generate))]
        [HarmonyPrefix]
        static void RuntimeDungeon_Pre_Generate(RuntimeDungeon __instance)
        {
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                switch (RoundManager.Instance.currentLevel.name)
                {
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
                                    __instance.Generator.LengthMultiplier = (2.2f / 0.9f) * RoundManager.Instance.mapSizeMultiplier;
                                    break;
                                default:
                                    __instance.Generator.LengthMultiplier *= 2f;
                                    break;
                            }
                        }
                        break;
                }
            }

            // just to avoid rounding errors with other clients
            __instance.Generator.LengthMultiplier = Mathf.Floor(__instance.Generator.LengthMultiplier * 100f) / 100f;

            Plugin.Logger.LogDebug($"Final length multiplier: {__instance.Generator.LengthMultiplier} (Interior #{RoundManager.Instance.currentDungeonType}, seed: {StartOfRound.Instance.randomMapSeed})");
        }
    }
}
