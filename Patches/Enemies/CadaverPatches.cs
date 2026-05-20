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
    }
}
