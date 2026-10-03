using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    static class PlayerControllerBPatches
    {
        [HarmonyPatch(nameof(PlayerControllerB.OnEnable))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_OnEnable()
        {
            Common.activateItem = InputSystem.actions.FindAction("ActivateItem", false);
        }

        [HarmonyPatch($"{nameof(IVisibleThreat)}.{nameof(IVisibleThreat.GetThreatLevel)}")]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_GetThreatLevel(PlayerControllerB __instance, ref int __result)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer is HauntedMaskItem hauntedMaskItem)
                __result += hauntedMaskItem.maskOn ? 4 : 2;
        }
    }
}
