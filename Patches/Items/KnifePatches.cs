using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(KnifeItem))]
    static class KnifePatches
    {
        static float knifeCooldown = 0.43f;
        static float timeAtLastSwing;
        static InputAction activateItem;

        [HarmonyPatch(nameof(KnifeItem.EquipItem))]
        [HarmonyPostfix]
        static void KnifeItem_Post_EquipItem(KnifeItem __instance)
        {
            knifeCooldown = BRBNetworker.Instance.KnifeShortCooldown.Value ? 0.37f : 0.43f;
            //Plugin.Logger.LogDebug($"Knife cooldown: {knifeCooldown}s");
        }

        [HarmonyPatch(nameof(KnifeItem.HitKnife))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> KnifeItem_Trans_HitKnife(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.43f)
                {
                    codes[i].opcode = OpCodes.Ldsfld;
                    codes[i].operand = AccessTools.Field(typeof(KnifePatches), nameof(knifeCooldown));
                    Plugin.Logger.LogDebug("Transpiler (Knife): Dynamic cooldown");
                    return codes;
                }
            }

            return instructions;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.OnEnable))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_OnEnable()
        {
            activateItem = InputSystem.actions.FindAction("ActivateItem", false);
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_Update(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.timeSinceSwitchingSlots >= 0.075f && __instance.currentlyHeldObjectServer is KnifeItem knifeItem && Configuration.knifeAutoSwing.Value && __instance.CanUseItem() && activateItem.IsPressed() && Time.realtimeSinceStartup - timeAtLastSwing > 0.12f && Time.realtimeSinceStartup - knifeItem.timeAtLastDamageDealt > knifeCooldown)
            {
                ShipBuildModeManager.Instance.CancelBuildMode();
                __instance.currentlyHeldObjectServer.UseItemOnClient();
                __instance.timeSinceSwitchingSlots = 0f;
            }
        }

        [HarmonyPatch(typeof(KnifeItem), nameof(KnifeItem.HitKnife))]
        [HarmonyPostfix]
        static void KnifeItem_Post_ItemActivate(KnifeItem __instance)
        {
            //Plugin.Logger.LogDebug($"Knife swung at {Time.realtimeSinceStartup}s ({Time.realtimeSinceStartup - timeAtLastSwing}s since last swing)");
            timeAtLastSwing = Time.realtimeSinceStartup + Random.Range(0f, 0.01f); // light variation in swing timing
        }
    }
}
