using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(KnifeItem))]
    internal class KnifePatches
    {
        static float knifeCooldown = 0.43f;
        static float timeAtLastSwing;
        static InputAction activateItem;

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        static void GrabbableObject_Post_Start(GrabbableObject __instance)
        {
            if (__instance.IsServer && !StartOfRound.Instance.inShipPhase && __instance.scrapValue == 35 && Configuration.butlerKnifePrice.Value && __instance is KnifeItem)
            {
                Plugin.Logger.LogInfo("Trying to sync knife price on server");
                BRBNetworker.Instance.SyncScrapPriceClientRpc(__instance.GetComponent<NetworkObject>(), Random.Range(28, 84));
            }
        }

        [HarmonyPatch(nameof(KnifeItem.EquipItem))]
        [HarmonyPostfix]
        static void KnifeItem_Post_EquipItem(KnifeItem __instance)
        {
            knifeCooldown = BRBNetworker.Instance.KnifeShortCooldown.Value ? 0.37f : 0.43f;
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
            activateItem = IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem", false);
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_Update(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.timeSinceSwitchingSlots >= 0.075f && __instance.currentlyHeldObjectServer is KnifeItem knifeItem && Configuration.knifeAutoSwing.Value && __instance.CanUseItem() && activateItem.IsPressed() && Time.realtimeSinceStartup - timeAtLastSwing > 0.2f && Time.realtimeSinceStartup - knifeItem.timeAtLastDamageDealt > knifeCooldown)
            {
                // prevents two swings when first clicking button
                if (Time.realtimeSinceStartup - timeAtLastSwing <= 0.5f)
                {
                    ShipBuildModeManager.Instance.CancelBuildMode();
                    __instance.currentlyHeldObjectServer.UseItemOnClient();
                    __instance.timeSinceSwitchingSlots = 0f;
                }
                timeAtLastSwing = Time.realtimeSinceStartup /*+ Random.Range(0f, 0.1f)*/;
            }
        }
    }
}
