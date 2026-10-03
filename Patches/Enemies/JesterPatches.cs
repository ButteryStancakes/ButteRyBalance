using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(JesterAI))]
    static class JesterPatches
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SwitchToBehaviourStateOnLocalClient))]
        [HarmonyPostfix]
        static void EnemyAI_Post_SwitchToBehaviourStateOnLocalClient(EnemyAI __instance, int stateIndex)
        {
            if (__instance is JesterAI jesterAI && stateIndex == 1 && BRBNetworker.Instance.JesterWalkThrough.Value)
                jesterAI.mainCollider.isTrigger = true;
        }

        [HarmonyPatch(nameof(JesterAI.SetJesterInitialValues))]
        [HarmonyPostfix]
        static void JesterAI_Post_SetJesterInitialValues(JesterAI __instance)
        {
            if (StartOfRound.Instance.connectedPlayersAmount < 4 && BRBNetworker.Instance.JesterLongCooldown.Value)
                __instance.beginCrankingTimer = Mathf.Max(__instance.beginCrankingTimer, Random.Range(12f, 28f));
        }

        [HarmonyPatch(nameof(JesterAI.Update))]
        [HarmonyPrefix]
        static void JesterAI_Pre_Update(JesterAI __instance)
        {
            if (__instance.currentBehaviourStateIndex == 2 && __instance.IsOwner && __instance.targetPlayer != null && __instance.targetPlayer.isPlayerControlled && __instance.targetPlayer.isInsideFactory)
            {
                if (__instance.noPlayersToChaseTimer <= 0f)
                    Plugin.Logger.LogDebug("Refreshed Jester's no players timer");

                __instance.noPlayersToChaseTimer = 5f;
            }
        }
    }
}
