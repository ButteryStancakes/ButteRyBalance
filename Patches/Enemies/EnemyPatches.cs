using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(EnemyAI))]
    static class EnemyPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.CheckLineOfSightForPlayer))]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.CheckLineOfSightForClosestPlayer))]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSightNonAlloc), [typeof(PlayerControllerB[]), typeof(float), typeof(int), typeof(Transform), typeof(float), typeof(int)])]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSightNonAlloc), [typeof(float), typeof(int), typeof(Transform), typeof(float), typeof(int)])]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSight))]
        [HarmonyPrefix]
        static void EnemyAI_Pre_LineOfSight(EnemyAI __instance, ref int range)
        {
            if (!__instance.isOutside && !__instance.enemyType.canSeeThroughFog && RoundManager.Instance.indoorFog.gameObject.activeSelf && range > Common.INTERIOR_FOG_RANGE && BRBNetworker.Instance.IndoorFogSight.Value)
                range = Common.INTERIOR_FOG_RANGE;
        }
    }
}
