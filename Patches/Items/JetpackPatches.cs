using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(JetpackItem))]
    internal class JetpackPatches
    {
        [HarmonyPatch(nameof(JetpackItem.EquipItem))]
        [HarmonyPostfix]
        static void JetpackItem_Post_EquipItem(JetpackItem __instance)
        {
            if (BRBNetworker.Instance.JetpackBattery.Value)
                __instance.itemProperties.batteryUsage = 40f;

            if (BRBNetworker.Instance.JetpackInertia.Value || Configuration.jetpackInertia.Value)
            {
                __instance.jetpackForceChangeSpeed = 1f;
                __instance.jetpackDeaccelleration = 75f;
            }
            else
                __instance.jetpackForceChangeSpeed = 50f;
        }

        [HarmonyPatch(typeof(JetpackItem), nameof(JetpackItem.Update))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> JetpackItem_Trans_Update(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 1; i < codes.Count - 3; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 50f && codes[i + 1].opcode == OpCodes.Mul)
                {
                    codes[i].opcode = OpCodes.Ldfld;
                    codes[i].operand = AccessTools.Field(typeof(JetpackItem), nameof(JetpackItem.jetpackForceChangeSpeed));
                    codes.Insert(i, new(OpCodes.Ldarg_0));
                    Plugin.Logger.LogDebug("Transpiler (Jetpack): Dynamic inertia");
                    return codes;
                }
            }

            return instructions;
        }
    }
}
