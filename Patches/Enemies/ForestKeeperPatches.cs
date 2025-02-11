using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    class ForestKeeperPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSight))]
        [HarmonyPrefix]
        static void EnemyAI_Pre_GetAllPlayersInLineOfSight(EnemyAI __instance, ref int range)
        {
            if (__instance.isOutside && !__instance.enemyType.canSeeThroughFog && range > 30 && __instance.IsOwner && __instance is ForestGiantAI && Configuration.giantSnowSight.Value && Common.IsSnowLevel())
                range = 30;
        }

        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSight))]
        [HarmonyPrefix]
        static void EnemyAI_Post_GetAllPlayersInLineOfSight(EnemyAI __instance, PlayerControllerB[] __result)
        {
            // when no players are in line of sight, forget about players you've seen before
            if (__result == null && __instance.IsOwner && __instance is ForestGiantAI forestGiantAI && Configuration.giantForgetTargets.Value)
            {
                for (int i = 0; i < forestGiantAI.playerStealthMeters.Length; i++)
                    forestGiantAI.playerStealthMeters[i] = Mathf.Clamp01(forestGiantAI.playerStealthMeters[i] - (0.33f * Time.deltaTime));
            }
        }
    }
}
