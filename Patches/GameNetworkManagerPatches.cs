using ButteRyBalance.Network;
using ButteRyBalance.Overrides;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    class GameNetworkManagerPatches
    {
        [HarmonyPatch(nameof(GameNetworkManager.Start))]
        [HarmonyPostfix]
        public static void GameNetworkManager_Post_Start()
        {
            BRBNetworker.Init();
        }

        [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
        [HarmonyPostfix]
        public static void GameNetworkManager_Post_Disconnect()
        {
            if (RoundManager.Instance != null && RoundManager.Instance.IsServer)
                InfestationOverrides.EndInfestation();
            Common.Disconnect();
        }
    }
}
