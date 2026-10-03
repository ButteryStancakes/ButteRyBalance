using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(SprayPaintItem))]
    static class SprayPaintPatches
    {
        internal static int damageNumber = 8;

        [HarmonyPatch(nameof(SprayPaintItem.HealPlayerInfection))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> SprayPaintItem_Trans_HealPlayerInfection(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            MethodInfo damagePlayer = AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer));
            for (int i = 0; i < codes.Count - 9; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_8 && codes[i + 9].opcode == OpCodes.Callvirt && codes[i + 9].operand as MethodInfo == damagePlayer)
                {
                    codes[i].opcode = OpCodes.Ldsfld;
                    codes[i].operand = AccessTools.Field(typeof(SprayPaintPatches), nameof(damageNumber));
                    Plugin.Logger.LogDebug("Transpiler (Spray paint): Dynamic damage");
                    return codes;
                }
            }

            return instructions;
        }

        [HarmonyPatch(nameof(SprayPaintItem.ResizeMatrix))]
        [HarmonyPrefix]
        static void SprayPaintItem_Pre_ResizeMatrix(ref float shrinkSpeed, bool shrink)
        {
            if (shrink && shrinkSpeed < 2.2f && BRBNetworker.Instance.VainsShrink.Value)
                shrinkSpeed = 2.2f;
        }
    }
}
