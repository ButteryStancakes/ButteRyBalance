using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(DressGirlAI))]
    class GirlPatches
    {
        static bool chasing;

        [HarmonyPatch(nameof(DressGirlAI.FlipLightsBreakerServerRpc))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DressGirlAI_Trans_FlipLightsBreakerServerRpc(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            MethodInfo messWithLightsClientRpc = AccessTools.Method(typeof(DressGirlAI), nameof(DressGirlAI.MessWithLightsClientRpc));
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == messWithLightsClientRpc)
                {
                    codes[i].operand = AccessTools.Method(typeof(GirlPatches), nameof(GirlFlipsBreakers));
                    Plugin.Logger.LogDebug($"Transpiler (Girl): Turn off breakers");
                    return codes;
                }
            }

            return instructions;
        }

        static void GirlFlipsBreakers(DressGirlAI dressGirlAI)
        {
            if (BRBNetworker.Instance != null && BRBNetworker.Instance.GirlBreakers.Value)
                dressGirlAI.FlipLightsBreakerClientRpc();
            else
                dressGirlAI.MessWithLightsClientRpc();
        }

        [HarmonyPatch(nameof(DressGirlAI.BeginChasing))]
        [HarmonyPrefix]
        static void DressGirlAI_Pre_BeginChasing(DressGirlAI __instance)
        {
            if (__instance.IsOwner)
                chasing = true;
        }

        [HarmonyPatch(nameof(DressGirlAI.BeginChasing))]
        [HarmonyPostfix]
        static void DressGirlAI_Post_BeginChasing(DressGirlAI __instance)
        {
            if (chasing)
            {
                RoundManager.Instance.FlickerLights(true, true);
                GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(__instance.timesSeenByPlayer > 0 ? 0.9f : 0.2f);
            }
            chasing = false;
        }

        [HarmonyPatch(nameof(DressGirlAI.FlipLightsBreakerServerRpc))]
        [HarmonyPostfix]
        static void DressGirlAI_Post_FlipLightsBreakerServerRpc(DressGirlAI __instance)
        {
            chasing = false;
        }

        [HarmonyPatch(nameof(DressGirlAI.Update))]
        [HarmonyPrefix]
        static void DressGirlAI_Pre_Update()
        {
            Common.girlUpdating = true;
        }

        [HarmonyPatch(nameof(DressGirlAI.Update))]
        [HarmonyPostfix]
        static void DressGirlAI_Post_Update()
        {
            Common.girlUpdating = false;
        }

        [HarmonyPatch(nameof(DressGirlAI.Start))]
        [HarmonyPostfix]
        static void DressGirlAI_Post_Start(DressGirlAI __instance)
        {
            Common.girl = __instance;
        }
    }
}
