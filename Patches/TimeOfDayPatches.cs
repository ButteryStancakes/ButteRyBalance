using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

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
                TimeOfDay.Instance.currentWeatherVariable = -16f;
                TimeOfDay.Instance.currentWeatherVariable2 = -5f;
            }
        }

        [HarmonyPatch(nameof(TimeOfDay.SetWeatherBasedOnVariables))]
        [HarmonyPostfix]
        static void TimeOfDay_Post_SetWeatherBasedOnVariables(TimeOfDay __instance)
        {
            if (StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.Foggy || !__instance.foggyWeather.enabled || !BRBNetworker.Instance.FoggyLimit.Value)
                return;

            __instance.foggyWeather.parameters.meanFreePath = Mathf.Max(__instance.foggyWeather.parameters.meanFreePath, 7f);
        }

        [HarmonyPatch(nameof(TimeOfDay.DecideRandomDayEvents))]
        [HarmonyPrefix]
        static bool TimeOfDay_Pre_DecideRandomDayEvents(TimeOfDay __instance)
        {
            return (!__instance.IsServer || !Configuration.experimentationNoEvents.Value || !BRBNetworker.Instance.MoonsKillSwitch.Value);
        }
    }
}
