using ButteRyBalance.Network;
using DunGen;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(RuntimeDungeon))]
    class RuntimeDungeonPatches
    {
        const float REND_DINE_DIFF = 1.2f / 1.3f;

        [HarmonyPatch(nameof(RuntimeDungeon.Generate))]
        [HarmonyPrefix]
        static void RuntimeDungeon_Pre_Generate(RuntimeDungeon __instance)
        {
            bool shrinkMineshafts = RoundManager.Instance.currentDungeonType == 4 && BRBNetworker.Instance.ShrinkMineshafts.Value;
            if (shrinkMineshafts)
            {
                __instance.Generator.LengthMultiplier *= 0.9f * 0.9f;
                Plugin.Logger.LogDebug("Invert mineshaft MapTileSize");
            }

            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                switch (RoundManager.Instance.currentLevel.name)
                {
                    case "MarchLevel":
                        if (BRBNetworker.Instance.MarchShrink.Value)
                        {
                            __instance.Generator.LengthMultiplier *= 0.9f;
                            Plugin.Logger.LogDebug("Shrunk March interior");
                        }
                        break;
                    case "DineLevel":
                        // if rend is generating 1.6x mineshafts...
                        if (BRBNetworker.Instance.RendShrink.Value && shrinkMineshafts)
                        {
                            // reduce dineshaft from 1.8x to 1.66x
                            __instance.Generator.LengthMultiplier *= REND_DINE_DIFF;
                            Plugin.Logger.LogDebug("Shrunk Dine mineshaft");
                        }
                        break;
                    case "TitanLevel":
                        if (shrinkMineshafts)
                        {
                            // 2.2x -> 2x
                            __instance.Generator.LengthMultiplier /= 1.1f;
                            Plugin.Logger.LogDebug("Shrunk Titan mineshaft");
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
