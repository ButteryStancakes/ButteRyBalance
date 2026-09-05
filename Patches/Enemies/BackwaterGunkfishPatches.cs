using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(StingrayAI))]
    static class BackwaterGunkfishPatches
    {
        [HarmonyPatch(nameof(StingrayAI.Start))]
        [HarmonyPostfix]
        static void StingrayAI_Post_Start(StingrayAI __instance)
        {
            if (BRBNetworker.Instance.GunkfishSquishy.Value)
                __instance.enemyHP = 3;
        }
    }
}