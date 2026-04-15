using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    static class VehicleControllerPatches
    {
        [HarmonyPatch(nameof(VehicleController.CarReactToObstacle))]
        [HarmonyPrefix]
        static bool VehicleController_Pre_CarReactToObstacle(VehicleController __instance, EnemyAI enemyScript)
        {
            return enemyScript == null || !BRBNetworker.Instance.FoxSlender.Value || enemyScript is not BushWolfEnemy;
        }
    }
}
