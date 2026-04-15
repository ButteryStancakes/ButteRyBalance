using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(LungProp))]
    static class ApparatusPatches
    {
        [HarmonyPatch(nameof(LungProp.EquipItem))]
        [HarmonyPrefix]
        static void LungProp_Pre_EquipItem(LungProp __instance)
        {
            if (__instance.IsOwner && __instance.isLungDocked && BRBNetworker.Instance.ApparatusPrice.Value)
                BRBNetworker.Instance.SyncScrapPriceRpc(__instance.NetworkObject, __instance.scrapValue); // to show value on scan
        }
    }
}
