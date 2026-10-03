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

            int changes = 0;
            for (int i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.NEXT)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4 && (int)codes[i - 1].operand == 200 && codes[i - 2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 2].operand == 70)
                    {
                        codes[i - 1].operand = 201;
                        Plugin.Logger.LogDebug($"Transpiler (Nest egg): 70-201");
                        changes++;
                    }
                    else if (codes[i - 1].opcode == OpCodes.Ldc_I4_S && codes[i - 2].opcode == OpCodes.Ldc_I4_S)
                    {
                        if ((sbyte)codes[i - 1].operand == 120 && (sbyte)codes[i - 2].operand == 70)
                        {
                            codes[i - 1].operand = (sbyte)121;
                            Plugin.Logger.LogDebug($"Transpiler (Nest egg): 70-121");
                            changes++;
                        }
                        else if ((sbyte)codes[i - 1].operand == 70 && (sbyte)codes[i - 2].operand == 40)
                        {
                            codes[i - 1].operand = (sbyte)71;
                            Plugin.Logger.LogDebug($"Transpiler (Nest egg): 40-71");
                            changes++;
                        }
                    }
                }
            }

            if (changes < 3)
                Plugin.Logger.LogError("Nest egg transpiler failed");

            return codes;
        }

        [HarmonyPatch(nameof(GiantKiwiAI.OnCollideWithEnemy))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> GiantKiwiAI_Trans_OnCollideWithEnemy(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = instructions.ToList();
            Label label = generator.DefineLabel();

            // adapted from KidnapperFoxPatches.BushWolfEnemy_Trans_OnCollideWithPlayer() (reference comments there)
            FieldInfo attacking = AccessTools.Field(typeof(GiantKiwiAI), nameof(GiantKiwiAI.attacking));
            bool checkSelfDead = false, checkTargetKillable = false, syncKill = false, syncDamage = false;
            for (int i = 5; i < codes.Count; i++)
            {
                if ((!checkSelfDead || !checkTargetKillable) && codes[i].opcode == OpCodes.Ldfld && (FieldInfo)codes[i].operand == attacking && codes[i - 1].opcode == OpCodes.Ldarg_0 && codes[i - 3].opcode == OpCodes.Brfalse)
                {
                    Label originalLabel = (Label)codes[i - 3].operand;
                    if (originalLabel != null && codes[i - 1].labels.Contains(originalLabel))
                    {
                        int insertTarget = i - 1;
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
                            Plugin.Logger.LogDebug("Transpiler (Giant Sapsucker collision): Check if self is dead");
                            checkSelfDead = true;
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
                            Plugin.Logger.LogDebug("Transpiler (Giant Sapsucker collision): Check if target is killable");
                            checkTargetKillable = true;
                        }
                    }
                }
                else if ((!syncKill || !syncDamage) && codes[i].opcode == OpCodes.Callvirt)
                {
                    MethodInfo methodInfo = codes[i].operand as MethodInfo;
                    if (!syncKill && methodInfo == ReflectionCache.KILL_ENEMY && codes[i - 2].opcode == OpCodes.Ldarg_2)
                    {
                        codes.InsertRange(i - 2,
                        [
                            new(OpCodes.Ldarg_2),
                            new(OpCodes.Call, ReflectionCache.IS_OWNER),
                            new(OpCodes.Brfalse, label)
                        ]);
                        i += 3;
                        codes[i].operand = ReflectionCache.KILL_ENEMY_ON_OWNER_CLIENT;
                        Plugin.Logger.LogDebug("Transpiler (Giant Sapsucker collision): Sync kill calls");
                        syncKill = true;
                    }
                    else if (!syncDamage && methodInfo == ReflectionCache.HIT_ENEMY && codes[i - 5].opcode == OpCodes.Ldarg_2)
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
                        Plugin.Logger.LogDebug("Transpiler (Giant Sapsucker collision): Sync damage calls");
                        syncDamage = true;
                    }
                }
            }

            if ((checkSelfDead || checkTargetKillable || syncKill || syncDamage) && codes[^1].opcode == OpCodes.Ret)
                codes[^1].labels.Add(label);

            if (checkSelfDead && checkTargetKillable && syncKill && syncDamage)
                return codes;

            Plugin.Logger.LogError("Giant Sapsucker collision transpiler failed");
            return instructions;
        }
    }
}
