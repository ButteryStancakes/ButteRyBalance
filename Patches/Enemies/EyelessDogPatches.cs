using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(MouthDogAI))]
    static class EyelessDogPatches
    {
        [HarmonyPatch(nameof(MouthDogAI.OnCollideWithEnemy))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> MouthDogAI_Trans_OnCollideWithEnemy(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = instructions.ToList();
            Label label = generator.DefineLabel();

            // adapted from KidnapperFoxPatches.BushWolfEnemy_Trans_OnCollideWithPlayer() (reference comments there)
            FieldInfo timeSinceHittingOtherEnemy = AccessTools.Field(typeof(MouthDogAI), nameof(MouthDogAI.timeSinceHittingOtherEnemy));
            bool checkSelfDead = false, checkTargetDead = false, checkTargetKillable = false, syncDamage = false;
            for (int i = 5; i < codes.Count; i++)
            {
                if ((!checkSelfDead || !checkTargetDead || !checkTargetKillable) && i < codes.Count - 4 && codes[i].opcode == OpCodes.Ldfld && (FieldInfo)codes[i].operand == timeSinceHittingOtherEnemy && codes[i + 1].opcode == OpCodes.Ldc_R4 && (float)codes[i + 1].operand == 1f && codes[i - 1].opcode == OpCodes.Ldarg_0 && codes[i + 2].opcode == OpCodes.Bge_Un)
                {
                    Label originalLabel = (Label)codes[i + 2].operand;
                    if (originalLabel != null && codes[i + 4].labels.Contains(originalLabel))
                    {
                        int insertTarget = i + 4;
                        if (!checkSelfDead)
                        {
                            codes[insertTarget] = new(codes[insertTarget].opcode, codes[insertTarget].operand);
                            CodeInstruction newJumpTarget = new(OpCodes.Ldarg_0);
                            newJumpTarget.labels.Add(originalLabel);
                            codes.InsertRange(insertTarget,
                            [
                                newJumpTarget,
                                new(OpCodes.Ldfld, ReflectionCache.IS_ENEMY_DEAD),
                                new(OpCodes.Brtrue, label)
                            ]);
                            insertTarget += 3;
                            Plugin.Logger.LogDebug("Transpiler (Eyeless dog collision): Check if self is dead");
                            checkSelfDead = true;
                        }
                        if (!checkTargetDead)
                        {
                            codes.InsertRange(insertTarget,
                            [
                                new(OpCodes.Ldarg_2),
                                new(OpCodes.Ldfld, ReflectionCache.IS_ENEMY_DEAD),
                                new(OpCodes.Brtrue, label)
                            ]);
                            insertTarget += 3;
                            Plugin.Logger.LogDebug("Transpiler (Eyeless dog collision): Check if target is dead");
                            checkTargetDead = true;
                        }
                        if (!checkTargetKillable)
                        {
                            codes.InsertRange(insertTarget,
                            [
                                new(OpCodes.Ldarg_2),
                                new(OpCodes.Ldfld, ReflectionCache.ENEMY_TYPE),
                                new(OpCodes.Ldfld, ReflectionCache.CAN_DIE),
                                new(OpCodes.Brfalse, label)
                            ]);
                            insertTarget += 4;
                            Plugin.Logger.LogDebug("Transpiler (Eyeless dog collision): Check if target is killable");
                            checkTargetKillable = true;
                        }
                    }
                }
                else if (!syncDamage && codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.HIT_ENEMY && codes[i - 5].opcode == OpCodes.Ldarg_2)
                {
                    codes.InsertRange(i - 5,
                    [
                        new(OpCodes.Ldarg_2),
                        new(OpCodes.Call, ReflectionCache.IS_OWNER),
                        new(OpCodes.Brfalse, label)
                    ]);
                    i += 3;
                    codes.Insert(i - 3, new(OpCodes.Call, ReflectionCache.ZERO));
                    i++;
                    codes[i].operand = ReflectionCache.HIT_ENEMY_ON_LOCAL_CLIENT;
                    Plugin.Logger.LogDebug("Transpiler (Eyeless dog collision): Sync damage calls");
                    syncDamage = true;
                }
            }

            if ((checkSelfDead || checkTargetDead || checkTargetKillable || syncDamage) && codes[^1].opcode == OpCodes.Ret)
                codes[^1].labels.Add(label);

            if (checkSelfDead && checkTargetDead && checkTargetKillable && syncDamage)
                return codes;

            Plugin.Logger.LogError("Eyeless dog collision transpiler failed");
            return instructions;
        }
    }
}
