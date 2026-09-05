using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Items
{
    [HarmonyPatch(typeof(LungProp))]
    static class ApparatusPatches
    {
        [HarmonyPatch(nameof(LungProp.EquipItem))]
        [HarmonyPrefix]
        static void LungProp_Pre_EquipItem(LungProp __instance, ref bool __state)
        {
            __state = __instance.isLungDocked;
        }

        [HarmonyPatch(nameof(LungProp.EquipItem))]
        [HarmonyPostfix]
        static void LungProp_Post_EquipItem(LungProp __instance, bool __state)
        {
            if (__state && !__instance.isLungDocked && BRBNetworker.Instance.ApparatusPrice.Value)
                __instance.SetScrapValue(__instance.scrapValue); // to show value on scan
        }
    }
}