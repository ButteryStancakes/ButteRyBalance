using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(SpringManAI))]
    static class CoilHeadPatches
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

        [HarmonyPatch(nameof(SpringManAI.Update))]
        [HarmonyPostfix]
        static void SpringManAI_Post_Update(SpringManAI __instance)
        {
            if (!__instance.isEnemyDead && !__instance.setOnCooldown && __instance.currentBehaviourStateIndex == 0)
            {
                // revert walk speed from v70, when animation made better sense
                if (__instance.creatureAnimator.GetFloat("walkSpeed") == 4.7f)
                    __instance.creatureAnimator.SetFloat("walkSpeed", 2.5f);
            }

            if (__instance.IsServer && Configuration.coilheadPersistence.Value)
                __instance.timeSpentMoving = 0f;
        }
    }
}
