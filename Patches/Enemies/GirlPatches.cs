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
    }
}
