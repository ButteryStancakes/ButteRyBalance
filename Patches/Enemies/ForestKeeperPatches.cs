using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    static class ForestKeeperPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.GetAllPlayersInLineOfSight))]
        [HarmonyPrefix]
        static void EnemyAI_Pre_GetAllPlayersInLineOfSight(EnemyAI __instance, ref int range)
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
            if (force == 12 && BRBNetworker.Instance.GiantSquishy.Value && (damageID == Common.DamageID.Cruiser || (damageID != Common.DamageID.Shovel && damageID != Common.DamageID.Knife)))
                force += 100;
        }
    }
}
