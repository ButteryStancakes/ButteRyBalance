using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(BushWolfEnemy))]
    static class KidnapperFoxPatches
    {
        [HarmonyPatch(typeof(BushWolfEnemy), nameof(BushWolfEnemy.Update))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BushWolfEnemy_Trans_Update(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && (FieldInfo)codes[i].operand == ReflectionCache.BACK_DOOR_OPEN && codes[i + 1].opcode == OpCodes.Brtrue)
                {
                    codes[i + 1].opcode = OpCodes.Brfalse;
                    codes[i] = new(OpCodes.Call, ReflectionCache.IS_PLAYER_SAFE_IN_BACK);
                    codes.InsertRange(i, [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, AccessTools.Field(typeof(EnemyAI), nameof(EnemyAI.targetPlayer))),
                    ]);
                    //i += 2;
                    Plugin.Logger.LogDebug($"Transpiler (Kidnapper fox): Patch backdoor protection");
                    return codes;
                }
            }

            return instructions;
        }
    }
}
