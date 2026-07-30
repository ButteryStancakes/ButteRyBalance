using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Compatibility
{
    static class ScandalsTweaksPatches
    {
        static Transform cabinWindow;
        static LayerMask vehicleMask = 1 << 30;

        [HarmonyPatch(typeof(ScandalsTweaks.Utils.Utilities), nameof(ScandalsTweaks.Utils.Utilities.ShouldAllowSightThroughVehicle))]
        [HarmonyPrefix]
        static bool Utilities_Pre_ShouldAllowSightForVehicle(PlayerControllerB player, EnemyAI enemy, ref bool __result)
        {
            if (BRBNetworker.Instance == null)
                return true;

            if (Common.vehicleController == null || Common.vehicleController.vehicleID != 0 || Common.INSTALLED_VERSION55_COMPANY_CRUISER)
                return true;

            if (!BRBNetworker.Instance.CruiserPatchEnemies.Value)
                return true;

            if (player.inVehicleAnimation && (Common.vehicleController.currentDriver == player || Common.vehicleController.currentPassenger == player))
            {
                if (enemy is RadMechAI radMechAI && (!radMechAI.isAlerted || radMechAI.focusedThreatTransform != player.transform) && radMechAI.currentBehaviourStateIndex != 2 && radMechAI.eye != null)
                {
                    if (cabinWindow == null)
                        cabinWindow = Common.vehicleController.transform.Find("Meshes/CabinWindowContainer");

                    // old bird is behind the cabin
                    if (cabinWindow != null && Vector3.Dot(cabinWindow.forward, (radMechAI.eye.position - cabinWindow.position).normalized) < 0f)
                    {
                        // old bird can't see player's head past the truck's body
                        if (Physics.Linecast(radMechAI.eye.position, player.gameplayCamera.transform.parent.position, vehicleMask, QueryTriggerInteraction.Ignore))
                            return true;
                    }
                }

                __result = true;
                return false;
            }

            return true;
        }
    }
}
