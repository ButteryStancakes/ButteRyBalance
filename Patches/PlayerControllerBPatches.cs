using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    static class PlayerControllerBPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.OnEnable))]
        [HarmonyPostfix]
        static void PlayerControllerB_Post_OnEnable()
        {
            Common.activateItem = InputSystem.actions.FindAction("ActivateItem", false);
        }
    }
}
