using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(BushWolfEnemy))]
    static class KidnapperFoxPatches
    {
        [HarmonyPatch(typeof(BushWolfEnemy), nameof(BushWolfEnemy.Update))]
        [HarmonyTranspiler]
        [HarmonyAfter(Plugin.GUID_SCANDALS_TWEAKS)]
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

        [HarmonyPatch(nameof(BushWolfEnemy.OnCollideWithEnemy))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BushWolfEnemy_Trans_OnCollideWithEnemy(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = instructions.ToList();
            Label label = generator.DefineLabel();

            FieldInfo timeSinceHitting = AccessTools.Field(typeof(BushWolfEnemy), nameof(BushWolfEnemy.timeSinceHitting));
            MethodInfo resetTrigger = AccessTools.Method(typeof(Animator), nameof(Animator.ResetTrigger), [typeof(string)]);
            bool checkTargetDead = false, checkTargetKillable = false, removeCopyPaste = false, syncDamage = false;
            int removeStart = -1, removeEnd = -1;
            for (int i = 5; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld && (FieldInfo)codes[i].operand == timeSinceHitting && codes[i - 1].opcode == OpCodes.Ldc_R4 && (float)codes[i - 1].operand == 0f && codes[i - 2].opcode == OpCodes.Ldarg_0 && codes[i - 4].opcode == OpCodes.Brfalse)
                {
                    Label originalLabel = (Label)codes[i - 4].operand;
                    if (originalLabel != null && codes[i - 2].labels.Contains(originalLabel))
                    {
                        if (!checkTargetDead)
                        {
                            codes[i - 2] = new(codes[i - 2].opcode, codes[i - 2].operand); // need to re-target the original jump, by removing the original label (new CodeInstruction, in case transpiler bails out)
                            CodeInstruction newJumpTarget = new(OpCodes.Ldarg_2);
                            newJumpTarget.labels.Add(originalLabel); // so it won't skip our new checks
                            codes.InsertRange(i - 2,
                            [
                                newJumpTarget,
                                new(OpCodes.Ldfld, ReflectionCache.IS_ENEMY_DEAD),
                                new(OpCodes.Brtrue, label)
                            ]);
                            i += 3;
                            Plugin.Logger.LogDebug("Transpiler (Kidnapper fox collision): Check if target is dead");
                            checkTargetDead = true;
                        }
                        if (!checkTargetKillable)
                        {
                            codes.InsertRange(i - 2,
                            [
                                new(OpCodes.Ldarg_2),
                                new(OpCodes.Ldfld, ReflectionCache.ENEMY_TYPE),
                                new(OpCodes.Ldfld, ReflectionCache.CAN_DIE),
                                new(OpCodes.Brfalse, label)
                            ]);
                            i += 4;
                            Plugin.Logger.LogDebug("Transpiler (Kidnapper fox collision): Check if target is killable");
                            checkTargetKillable = true;
                        }
                    }
                    if (!removeCopyPaste && removeStart < 0 && codes[i + 4].opcode == OpCodes.Callvirt && codes[i + 4].operand as MethodInfo == resetTrigger)
                        removeStart = i + 1;
                }
                else if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.HIT_ENEMY && codes[i - 5].opcode == OpCodes.Ldarg_2)
                {
                    if (!removeCopyPaste && removeEnd < 0)
                        removeEnd = i - 5;

                    if (!syncDamage)
                    {
                        codes.InsertRange(i - 5,
                        [
                            new(OpCodes.Ldarg_2),
                            new(OpCodes.Call, ReflectionCache.IS_OWNER),
                            new(OpCodes.Brfalse, label)
                        ]);
                        i += 3;
                        // TODO: need to find some way to define locals in a transpiler, to do it the "right way" (ILGenerator.DeclareLocal()?)
                        /*codes.InsertRange(i - 3,
                        [
                            new(OpCodes.Ldloca_S, (byte)0),
                            new(OpCodes.Initobj, typeof(Vector3)),
                            new(OpCodes.Ldloc_S, (byte)0),
                        ]);
                        i += 3;*/
                        codes.Insert(i - 3, new(OpCodes.Call, ReflectionCache.ZERO));
                        i++;
                        codes[i].operand = ReflectionCache.HIT_ENEMY_ON_LOCAL_CLIENT;
                        Plugin.Logger.LogDebug("Transpiler (Kidnapper fox collision): Sync damage calls");
                        syncDamage = true;
                    }
                }
            }

            if (!removeCopyPaste && removeStart >= 0 && removeStart < codes.Count && removeEnd >= 0 && removeEnd < codes.Count && removeEnd >= removeStart)
            {
                for (int i = removeStart; i < removeEnd; i++)
                    codes[i].opcode = OpCodes.Nop;
                Plugin.Logger.LogDebug("Transpiler (Kidnapper fox collision): Remove unused code");
                removeCopyPaste = true;
            }

            if ((checkTargetDead || checkTargetKillable || syncDamage) && codes[^1].opcode == OpCodes.Ret)
                codes[^1].labels.Add(label);

            if (checkTargetDead && checkTargetKillable && removeCopyPaste && syncDamage)
                return codes;

            Plugin.Logger.LogError("Kidnapper fox collision transpiler failed");
            return instructions;
        }
    }
}
