using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(BaboonBirdAI))]
    static class BaboonHawkPatches
    {
        [HarmonyPatch(nameof(BaboonBirdAI.OnCollideWithEnemy))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BaboonBirdAI_Trans_OnCollideWithEnemy(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = instructions.ToList();
            Label label = generator.DefineLabel();

            // adapted from KidnapperFoxPatches.BushWolfEnemy_Trans_OnCollideWithPlayer() (reference comments there)
            bool checkTargetDead = false, syncDamage = false;
            for (int i = 5; i < codes.Count; i++)
            {
                if (!checkTargetDead && i < codes.Count - 4 && codes[i].opcode == OpCodes.Ldarg_2 && codes[i + 1].opcode == OpCodes.Ldfld && (FieldInfo)codes[i + 1].operand == ReflectionCache.ENEMY_TYPE && codes[i + 2].opcode == OpCodes.Ldfld && (FieldInfo)codes[i + 2].operand == ReflectionCache.CAN_DIE && codes[i - 2].opcode == OpCodes.Brfalse)
                {
                    Label originalLabel = (Label)codes[i - 2].operand;
                    if (originalLabel != null && codes[i].labels.Contains(originalLabel))
                    {
                        codes[i] = new(codes[i].opcode, codes[i].operand);
                        CodeInstruction newJumpTarget = new(OpCodes.Ldarg_2);
                        newJumpTarget.labels.Add(originalLabel);
                        codes.InsertRange(i,
                        [
                            newJumpTarget,
                            new(OpCodes.Ldfld, ReflectionCache.IS_ENEMY_DEAD),
                            new(OpCodes.Brtrue, label)
                        ]);
                        i += 3;
                        if (codes[^1].opcode == OpCodes.Ret)
                            codes[^1].labels.Add(label);
                        Plugin.Logger.LogDebug("Transpiler (Baboon hawk collision): Check if target is dead");
                        checkTargetDead = true;
                    }
                }
                else if (!syncDamage && codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.HIT_ENEMY && codes[i - 5].opcode == OpCodes.Ldarg_2)
                {
                    codes.Insert(i - 3, new(OpCodes.Call, ReflectionCache.ZERO));
                    i++;
                    codes[i].operand = ReflectionCache.HIT_ENEMY_ON_LOCAL_CLIENT;
                    Plugin.Logger.LogDebug("Transpiler (Baboon hawk collision): Sync damage calls");
                    syncDamage = true;
                }
            }


            if (checkTargetDead && syncDamage)
                return codes;

            Plugin.Logger.LogError("Baboon hawk collision transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(BaboonBirdAI.CanGrabScrap))]
        [HarmonyPostfix]
        static void BaboonBirdAI_Post_CanGrabScrap(GrabbableObject scrap, ref bool __result)
        {
            // don't pick up maneaters or masks
            if (__result && (scrap.itemProperties.itemId == 123984 || scrap.GetComponent<HauntedMaskItem>() != null))
                __result = false;
        }
    }
}