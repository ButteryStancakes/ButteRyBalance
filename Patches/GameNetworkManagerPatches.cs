using ButteRyBalance.Network;
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
            Common.Disconnect();
        }
    }
}
