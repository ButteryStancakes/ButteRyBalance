using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;

namespace ButteRyBalance.Compatibility
{
    static class ScandalsTweaksPatches
    {
        [HarmonyPatch(typeof(ScandalsTweaks.Utils.Utilities), nameof(ScandalsTweaks.Utils.Utilities.ShouldAllowSightForVehicle))]
        [HarmonyPrefix]
        static bool Utilities_Pre_ShouldAllowSightForVehicle(PlayerControllerB player, ref bool __result)
        {
            if (BRBNetworker.Instance == null)
                return true;

            if (Common.vehicleController == null || Common.vehicleController.vehicleID != 0)
                return true;

            if (!BRBNetworker.Instance.CruiserPatchEnemies.Value)
                return true;

            if (player.inVehicleAnimation && (Common.vehicleController.currentDriver == player || Common.vehicleController.currentPassenger == player))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
