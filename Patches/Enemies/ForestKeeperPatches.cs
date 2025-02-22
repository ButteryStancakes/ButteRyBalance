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
    }
}
