using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(GiantKiwiAI))]
    class GiantSapsuckerPatches
    {
        [HarmonyPatch(nameof(GiantKiwiAI.SpawnNestEggs))]
        [HarmonyTranspiler]
        [HarmonyPriority(Priority.VeryLow)]
        static IEnumerable<CodeInstruction> GiantKiwiAI_Trans_SpawnNestEggs(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.NEXT)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4 && (int)codes[i - 1].operand == 200 && codes[i - 2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 2].operand == 70)
                    {
                        codes[i - 1].operand = 201;
                        Plugin.Logger.LogDebug($"Transpiler (Nest egg): 70-201");
                    }
                    else if (codes[i - 1].opcode == OpCodes.Ldc_I4_S && codes[i - 2].opcode == OpCodes.Ldc_I4_S)
                    {
                        if ((sbyte)codes[i - 1].operand == 120 && (sbyte)codes[i - 2].operand == 70)
                        {
                            codes[i - 1].operand = (sbyte)121;
                            Plugin.Logger.LogDebug($"Transpiler (Nest egg): 70-121");
                        }
                        else if ((sbyte)codes[i - 1].operand == 70 && (sbyte)codes[i - 2].operand == 40)
                        {
                            codes[i - 1].operand = (sbyte)71;
                            Plugin.Logger.LogDebug($"Transpiler (Nest egg): 40-71");
                        }
                    }
                }
            }

            //Plugin.Logger.LogError("Nest egg transpiler failed");
            return codes; // instructions
        }
    }
}
