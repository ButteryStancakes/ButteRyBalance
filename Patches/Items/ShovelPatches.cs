using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(Shovel))]
    static class ShovelPatches
    {
        static float timeLastBuffered;
        static InputAction activateItem;

        [HarmonyPatch(nameof(Shovel.ItemActivate))]
        [HarmonyPrefix]
        static void Shovel_Post_ItemActivate(Shovel __instance, bool buttonDown)
        {
            if (buttonDown && __instance.reelingUp)
                timeLastBuffered = Time.realtimeSinceStartup;
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
            if (__instance.currentlyHeldObjectServer != null && __instance.timeSinceSwitchingSlots >= 0.075f && __instance.currentlyHeldObjectServer is Shovel shovel && Configuration.shovelBuffer.Value && __instance.CanUseItem() && activateItem.IsPressed() && Time.realtimeSinceStartup - timeLastBuffered < 0.5f)
            {
                // prevents two swings when first clicking button
                if (Time.realtimeSinceStartup - timeLastBuffered <= 0.5f)
                {
                    ShipBuildModeManager.Instance.CancelBuildMode();
                    __instance.currentlyHeldObjectServer.UseItemOnClient();
                    __instance.timeSinceSwitchingSlots = 0f;
                }
                timeLastBuffered = Time.realtimeSinceStartup + Random.Range(0f, 0.01f);
            }
        }
    }
}
