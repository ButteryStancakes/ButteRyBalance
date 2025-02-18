using ButteRyBalance.Network;
using HarmonyLib;
using Unity.Netcode;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(LungProp))]
    internal class ApparatusPatches
    {
        [HarmonyPatch(nameof(LungProp.EquipItem))]
        [HarmonyPrefix]
        static void LungProp_Pre_EquipItem(LungProp __instance)
        {
            if (__instance.IsOwner && __instance.isLungDocked && __instance.scrapValue == 80 && BRBNetworker.Instance.ApparatusPrice.Value)
                BRBNetworker.Instance.SyncScrapPriceServerRpc(__instance.GetComponent<NetworkObject>(), new System.Random(StartOfRound.Instance.randomMapSeed).Next(40, 131));
        }
    }
}
