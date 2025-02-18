using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    class PlayerControllerBPatches
    {
        static bool wasCrouching;

        [HarmonyPatch(nameof(PlayerControllerB.UpdatePlayerAnimationsToOtherClients))]
        [HarmonyPrefix]
        public static void PlayerControllerB_Pre_UpdatePlayerAnimationsToOtherClients(PlayerControllerB __instance, ref float __state)
        {
            __state = __instance.updatePlayerAnimationsInterval;
        }

        [HarmonyPatch(nameof(PlayerControllerB.UpdatePlayerAnimationsToOtherClients))]
        [HarmonyPostfix]
        public static void PlayerControllerB_Post_UpdatePlayerAnimationsToOtherClients(PlayerControllerB __instance, float __state)
        {
            if (!__instance.IsOwner)
                return;

            if (__instance.updatePlayerAnimationsInterval < __state)
            {
                if (__instance.isCrouching != wasCrouching)
                {
                    wasCrouching = __instance.isCrouching;
                    BRBNetworker.Instance.SyncCrouchingServerRpc((int)__instance.playerClientId, __instance.isCrouching);
                }
            }
        }
    }
}
