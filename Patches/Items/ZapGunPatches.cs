using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(PatcherTool))]
    internal class ZapGunPatches
    {
        [HarmonyPatch(nameof(PatcherTool.EquipItem))]
        [HarmonyPostfix]
        static void PatcherTool_Post_EquipItem(PatcherTool __instance)
        {
            if (BRBNetworker.Instance.ZapGunBattery.Value)
                __instance.itemProperties.batteryUsage = 120f;
        }
    }
}
