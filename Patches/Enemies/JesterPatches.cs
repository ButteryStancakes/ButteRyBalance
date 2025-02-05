using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(JesterAI))]
    class JesterPatches
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
        static void JesterAI_PostSetJesterInitialValues(JesterAI __instance)
        {
            if (StartOfRound.Instance.connectedPlayersAmount < 4 && BRBNetworker.Instance.JesterLongCooldown.Value)
                __instance.beginCrankingTimer = Mathf.Max(__instance.beginCrankingTimer, Random.Range(12f, 28f));
        }
    }
}
