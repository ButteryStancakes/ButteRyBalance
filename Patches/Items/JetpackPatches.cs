using ButteRyBalance.Network;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(JetpackItem))]
    static class JetpackPatches
    {
        static readonly AnimationCurve accelerationCurve = new(
            new(1.5f, 10f),
            new(1.73f, 10f),
            new(2f, 4f),
            new(2.3f, 2f));
        static readonly AnimationCurve handlingCurve = new(
            new(1.5f, 20f),
            new(1.73f, 20f),
            new(2f, 35f),
            new(2.3f, 50f));
        static readonly AnimationCurve decelerationCurve = new(
            new(1.5f, 75f),
            new(2f, 50f),
            new(2.23f, 45f));

        static float timeSpentFlying;

        [HarmonyPatch(nameof(JetpackItem.EquipItem))]
        [HarmonyPostfix]
        static void JetpackItem_Post_EquipItem(JetpackItem __instance)
        {
            if (BRBNetworker.Instance.JetpackBattery.Value)
                __instance.itemProperties.batteryUsage = 40f;

            Configuration.JetpackControls jetpackControls = (Configuration.JetpackControls)BRBNetworker.Instance.JetpackControls.Value;
            if (jetpackControls == Configuration.JetpackControls.V49)
            {
                __instance.jetpackForceChangeSpeed = 1f;
                __instance.jetpackDeaccelleration = 75f;
            }
            else if (jetpackControls == Configuration.JetpackControls.Vanilla)
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

        [HarmonyPatch(nameof(JetpackItem.Update))]
        [HarmonyPrefix]
        static void JetpackItem_Pre_Update(JetpackItem __instance)
        {
            if (__instance.playerHeldBy == null || __instance.playerHeldBy != GameNetworkManager.Instance?.localPlayerController)
                return;

            float acceleration = 10f;
            if ((Configuration.JetpackControls)BRBNetworker.Instance.JetpackControls.Value == Configuration.JetpackControls.Dynamic)
            {
                acceleration = accelerationCurve.Evaluate(__instance.playerHeldBy.carryWeight);
                __instance.jetpackForceChangeSpeed = handlingCurve.Evaluate(__instance.playerHeldBy.carryWeight);
                __instance.jetpackDeaccelleration = decelerationCurve.Evaluate(__instance.playerHeldBy.carryWeight);
            }

            if (BRBNetworker.Instance.JetpackWarmUp.Value)
            {
                if (!__instance.playerHeldBy.jetpackControls)
                    timeSpentFlying = 0f;
                else if (__instance.jetpackActivated)
                    timeSpentFlying += Time.deltaTime;

                if (timeSpentFlying < 2f)
                    __instance.jetpackAcceleration = 2f * __instance.verticalMultiplier;
                else
                    __instance.jetpackAcceleration = Mathf.Lerp(2f, acceleration, Mathf.InverseLerp(2f, 4f, timeSpentFlying));
            }
            else
                __instance.jetpackAcceleration = acceleration;
        }
    }
}
