using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(HoarderBugAI))]
    static class HoardingBugPatches
    {
        [HarmonyPatch(nameof(HoarderBugAI.Update))]
        [HarmonyPostfix]
        static void HoarderBugAI_Post_Update(HoarderBugAI __instance)
        {
            if (__instance.IsOwner && Configuration.hoarderAngerManagement.Value && __instance.angryTimer > 0f)
                __instance.annoyanceMeter = 0f;
        }
    }
}
