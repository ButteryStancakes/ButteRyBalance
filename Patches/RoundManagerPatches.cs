using ButteRyBalance.Overrides;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    class RoundManagerPatches
    {
        [HarmonyPatch(nameof(RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies))]
        [HarmonyPrefix]
        static void RoundManager_Pre_AdvanceHourAndSpawnNewBatchOfEnemies(RoundManager __instance)
        {
            if (__instance.IsServer)
                OverrideCoordinator.BeforeSpawnWave();
        }

        [HarmonyPatch(nameof(RoundManager.RefreshEnemiesList))]
        [HarmonyPostfix]
        static void RoundManager_Post_RefreshEnemiesList()
        {
            OverrideCoordinator.AfterEnemyRefresh();
        }

        [HarmonyPatch(nameof(RoundManager.GenerateNewLevelClientRpc))]
        [HarmonyPrefix]
        static void RoundManager_Pre_GenerateNewLevelClientRpc()
        {
            OverrideCoordinator.BeforeGeneratingLevel();
        }
    }
}
