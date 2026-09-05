using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(RedLocustBees))]
    class CircuitBeePatches
    {
        [HarmonyPatch(nameof(RedLocustBees.SpawnHiveNearEnemy))]
        [HarmonyTranspiler]
        [HarmonyPriority(Priority.VeryLow)]
        static IEnumerable<CodeInstruction> RedLocustBees_Trans_SpawnHiveEnemy(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.NEXT)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 1].operand == 100 && codes[i - 2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 2].operand == 40)
                    {
                        codes[i - 1].operand = (sbyte)101;
                        Plugin.Logger.LogDebug($"Transpiler (Bee hive): 40-101");
                    }
                    else if (codes[i - 1].opcode == OpCodes.Ldc_I4 && (int)codes[i - 1].operand == 150 && codes[i - 2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 2].operand == 50)
                    {
                        codes[i - 1].operand = 151;
                        Plugin.Logger.LogDebug($"Transpiler (Bee hive): 50-151");
                    }
                }
            }

            //Plugin.Logger.LogError("Bee hive transpiler failed");
            return codes; // instructions
        }
    }
}
