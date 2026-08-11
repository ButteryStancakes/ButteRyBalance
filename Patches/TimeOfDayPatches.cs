using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    static class TimeOfDayPatches
    {
        [HarmonyPatch(nameof(TimeOfDay.SetWeatherBasedOnVariables))]
        [HarmonyPrefix]
        static void TimeOfDay_Pre_SetWeatherBasedOnVariables(TimeOfDay __instance)
        {
            if (!StartOfRound.Instance.isChallengeFile && !BRBNetworker.Instance.MoonsKillSwitch.Value && RoundManager.Instance.currentLevel.name == "DineLevel" && __instance.currentLevelWeather == LevelWeatherType.Flooded && BRBNetworker.Instance.DineFloods.Value)
            {
                // use v50 beta values since main entrance was moved down in v60
                //TimeOfDay.Instance.currentWeatherVariable = -16f;
                //TimeOfDay.Instance.currentWeatherVariable2 = -5f;

                // v49
                TimeOfDay.Instance.currentWeatherVariable = -21f;
                TimeOfDay.Instance.currentWeatherVariable2 = 12f;
            }
        }

        [HarmonyPatch(nameof(TimeOfDay.DecideRandomDayEvents))]
        [HarmonyPrefix]
        static bool TimeOfDay_Pre_DecideRandomDayEvents(TimeOfDay __instance)
        {
            return (!__instance.IsServer || !Configuration.experimentationNoEvents.Value || !BRBNetworker.Instance.MoonsKillSwitch.Value);
        }
    }
}
