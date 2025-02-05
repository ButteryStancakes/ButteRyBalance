using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(SpringManAI))]
    class CoilHeadPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SetEnemyStunned))]
        [HarmonyPrefix]
        static void EnemyAI_Pre_SetEnemyStunned(EnemyAI __instance, float setToStunTime)
        {
            if (__instance is SpringManAI springManAI && __instance.IsServer && Configuration.coilheadStunReset.Value && springManAI.onCooldownPhase <= 0f && setToStunTime > 0.5f)
            {
                springManAI.onCooldownPhase = setToStunTime * __instance.enemyType.stunTimeMultiplier;
                springManAI.setOnCooldown = true;
                springManAI.inCooldownAnimation = true;
                springManAI.SetCoilheadOnCooldownServerRpc(true);
            }
        }

        [HarmonyPatch(nameof(SpringManAI.SetCoilheadOnCooldownClientRpc))]
        [HarmonyPostfix]
        static void SpringManAI_Post_SetCoilheadOnCooldownClientRpc(SpringManAI __instance, bool setTrue)
        {
            if (setTrue)
                __instance.onCooldownPhase = Mathf.Max(__instance.onCooldownPhase, __instance.stunNormalizedTimer * __instance.enemyType.stunTimeMultiplier);
        }
    }
}
