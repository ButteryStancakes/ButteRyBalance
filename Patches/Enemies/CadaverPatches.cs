using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.VFX;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch]
    static class CadaverPatches
    {
        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.InfectPlayer))]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        static void CadaverGrowthAI_Post_InfectPlayer(CadaverGrowthAI __instance, PlayerControllerB playerScript/*, bool emittingSpores*/)
        {
            if (__instance.playerInfections[(int)playerScript.playerClientId].faceSpores == null /*&& emittingSpores*/)
                __instance.playerInfections[(int)playerScript.playerClientId].faceSpores = Object.Instantiate(__instance.faceSporesPrefab, RoundManager.Instance.mapPropsContainer?.transform, true).GetComponent<VisualEffect>();
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.DoAIInterval))]
        [HarmonyPrefix]
        static void CadaverGrowthAI_Pre_DoAIInterval(CadaverGrowthAI __instance, ref bool __state)
        {
            __state = __instance.inGrowthBurst;
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.DoAIInterval))]
        [HarmonyPostfix]
        static void CadaverGrowthAI_Post_DoAIInterval(CadaverGrowthAI __instance, bool __state)
        {
            if (__state && !__instance.inGrowthBurst && !Configuration.cadaversLimitGrowth.Value)
                __instance.growthBurstTimer = 0f;
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.Start))]
        [HarmonyPostfix]
        static void CadaverGrowthAI_Post_Start(CadaverGrowthAI __instance)
        {
            if (Configuration.cadaversLimitGrowth.Value)
                __instance.GrowthChancePerInterval = 20f;
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.RemoveWeedFromTile))]
        [HarmonyPrefix]
        static void CadaverGrowthAI_Pre_RemoveWeedFromTile(CadaverGrowthAI __instance, ref float __state)
        {
            __state = __instance.spreadInterval;
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.RemoveWeedFromTile))]
        [HarmonyPostfix]
        static void CadaverGrowthAI_Post_RemoveWeedFromTile(CadaverGrowthAI __instance, float __state)
        {
            if (Configuration.cadaversLimitGrowth.Value)
                __instance.spreadInterval = Mathf.Clamp(__state - 2f, -12f, __instance.spreadInterval);
        }
    }
}
