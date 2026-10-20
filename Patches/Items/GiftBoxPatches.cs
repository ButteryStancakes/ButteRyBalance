using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(GiftBoxItem))]
    class GiftBoxPatches
    {
        [HarmonyPatch(nameof(GiftBoxItem.InitializeAfterPositioning))]
        [HarmonyPatch(nameof(GiftBoxItem.LoadItemSaveData))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> GiftBoxItem_Trans(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 4; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == ReflectionCache.NEXT && codes[i - 1].opcode == OpCodes.Add && codes[i - 3].opcode == OpCodes.Ldfld && (FieldInfo)codes[i - 3].operand == ReflectionCache.MAX_VALUE && codes[i - 2].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i - 2].operand == 35)
                {
                    codes[i - 2].operand = (sbyte)((sbyte)codes[i - 2].operand + 1);
                    Plugin.Logger.LogDebug($"Transpiler ({__originalMethod.DeclaringType}.{__originalMethod.Name}): 25-{(sbyte)codes[i - 2].operand}");
                    return codes;
                }
            }

            Plugin.Logger.LogError($"{__originalMethod.DeclaringType}.{__originalMethod.Name} transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(GiftBoxItem.InitializeAfterPositioning))]
        [HarmonyPatch(nameof(GiftBoxItem.LoadItemSaveData))]
        [HarmonyPostfix]
        static void GiftBoxItem_Post(GiftBoxItem __instance)
        {
            if (__instance.IsServer && Configuration.giftBoost.Value && __instance.objectInPresentItem != null)
            {
                int temp = __instance.objectInPresentValue;
                System.Random giftRandom = new((int)__instance.targetFloorPosition.x + (int)__instance.targetFloorPosition.y);
                __instance.objectInPresentValue = (int)(giftRandom.Next(__instance.objectInPresentItem.minValue, __instance.objectInPresentItem.maxValue + 1) * RoundManager.Instance.scrapValueMultiplier) + giftRandom.Next(25, 36);
                Plugin.Logger.LogDebug($"Boosted value of Gift #{__instance.NetworkObjectId} ({__instance.objectInPresentItem.itemName}): ${temp} -> ${__instance.objectInPresentValue}");
            }
        }
    }
}
