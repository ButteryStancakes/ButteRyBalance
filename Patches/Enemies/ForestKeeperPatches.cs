using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    static class ForestKeeperPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSightNonAlloc), [typeof(float), typeof(int), typeof(Transform), typeof(float), typeof(int)])]
        [HarmonyPrefix]
        static void EnemyAI_Pre_GetAllPlayersInLineOfSightNonAlloc(EnemyAI __instance, ref int range)
        {
            if (__instance.isOutside && !__instance.enemyType.canSeeThroughFog && range > 30 && __instance.IsOwner && __instance is ForestGiantAI && Configuration.giantSnowSight.Value && Common.IsSnowLevel())
                range = 30;
        }

        [HarmonyPatch(nameof(ForestGiantAI.HitEnemy))]
        [HarmonyPrefix]
        static void ForestGiantAI_Pre_HitEnemy(ref int force, int hitID)
        {
            Common.DamageID damageID = (Common.DamageID)hitID;
            // instant death from cruiser damage
            if (force == 12 && BRBNetworker.Instance.GiantSquishy.Value && (damageID == Common.DamageID.Cruiser || (!Common.INSTALLED_CRUISER_IMPROVED && damageID != Common.DamageID.Shovel && damageID != Common.DamageID.Knife && Common.vehicleController != null)))
                force += 100;
        }
    }
}
