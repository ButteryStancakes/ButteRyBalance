using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(TetraChemicalItem))]
    internal class TZPPatches
    {
        static float tzpCapacity = 22f;

        [HarmonyPatch(nameof(TetraChemicalItem.EquipItem))]
        [HarmonyPostfix]
        static void TetraChemicalItem_Post_EquipItem(KnifeItem __instance)
        {
            tzpCapacity = BRBNetworker.Instance.TZPExpandCapacity.Value ? 34f : 22f;
        }

        [HarmonyPatch(nameof(TetraChemicalItem.Update))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> TetraChemicalItem_Trans_Update(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 22f)
                {
                    codes[i].opcode = OpCodes.Ldsfld;
                    codes[i].operand = AccessTools.Field(typeof(TZPPatches), nameof(tzpCapacity));
                    Plugin.Logger.LogDebug("Transpiler (TZP): Dynamic capacity");
                    return codes;
                }
            }

            return instructions;
        }
    }
}
