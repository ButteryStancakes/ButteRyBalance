using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(MoldSpreadManager))]
    static class MoldSpreadManagerPatches
    {
        [HarmonyPatch(nameof(MoldSpreadManager.GenerateMold))]
        [HarmonyPrefix]
        static void MoldSpreadManager_Pre_GenerateMold(ref int iterations)
        {
            if (BRBNetworker.Instance != null && iterations > BRBNetworker.Instance.VainsIterations.Value)
            {
                iterations = BRBNetworker.Instance.VainsIterations.Value;
                Plugin.Logger.LogDebug($"Vain Shrouds: Capped iterations at {BRBNetworker.Instance.VainsIterations.Value}");
            }
        }
    }
}
