using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(KnifeItem))]
    static class KnifePatches
    {
        static float knifeCooldown = 0.43f;
        static float timeAtLastSwing;

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        static void GrabbableObject_Post_Start(GrabbableObject __instance)
        {
            if (__instance.IsServer && !StartOfRound.Instance.inShipPhase && __instance.scrapValue == 35 && Configuration.butlerKnifePrice.Value && Common.butlerKnives.Count > 0 && __instance is KnifeItem)
            {
                Plugin.Logger.LogDebug("Trying to sync knife price on server");

                Vector2 pos = new(__instance.transform.position.x, __instance.transform.position.z);
                float nearest = float.MaxValue;
                ButlerEnemyAI dropper = null;
                foreach (KeyValuePair<ButlerEnemyAI, int> butlerKnife in Common.butlerKnives)
                {
                    if (!butlerKnife.Key.isEnemyDead)
                        continue;

                    // find the closest dead butler to the knife that was just dropped
                    float dist = Vector2.Distance(pos, new(butlerKnife.Key.transform.position.x, butlerKnife.Key.transform.position.z));
                    if (dist < nearest)
                    {
                        dropper = butlerKnife.Key;
                        nearest = dist;
                    }
                }

                if (dropper != null)
                {
                    Plugin.Logger.LogDebug($"Knife #{__instance.NetworkObjectId} is presumed to have dropped from Butler #{dropper.NetworkObjectId}; setting price $35 -> ${Common.butlerKnives[dropper]}");
                    BRBNetworker.Instance.SyncScrapPriceRpc(__instance.NetworkObject, Common.butlerKnives[dropper]);
                    Common.butlerKnives.Remove(dropper);
                }
                else
                    Plugin.Logger.LogWarning($"Could not find defined random price for knife #{__instance.NetworkObjectId}");
            }
        }

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

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_Update(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.timeSinceSwitchingSlots >= 0.075f && __instance.currentlyHeldObjectServer is KnifeItem knifeItem && Configuration.knifeAutoSwing.Value && __instance.CanUseItem() && Common.activateItem != null && Common.activateItem.IsPressed() && Time.realtimeSinceStartup - timeAtLastSwing > 0.12f && Time.realtimeSinceStartup - knifeItem.timeAtLastDamageDealt > knifeCooldown)
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
