using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    class TimeOfDayPatches
    {
        [HarmonyPatch(nameof(TimeOfDay.SetWeatherBasedOnVariables))]
        [HarmonyPostfix]
        static void TimeOfDay_Post_SetWeatherBasedOnVariables(TimeOfDay __instance)
        {
            string fogPath = "/Environment/Lighting/BrightDay/Local Volumetric Fog";

            if (StartOfRound.Instance.currentLevel.sceneName == "Level5Rend")
            {
                GameObject localVolumetricFog2 = GameObject.Find(fogPath + " (2)");
                // this fog is placed randomly next to the ship and makes the fire exit more dangerous than it needs to be
                if (localVolumetricFog2 != null)
                    localVolumetricFog2.SetActive(false);
                return;
            }

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

        [HarmonyPatch(nameof(TimeOfDay.CalculateLuckValue))]
        [HarmonyPrefix]
        static void TimeOfDay_Pre_CalculateLuckValue(TimeOfDay __instance)
        {
            for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[i].luckValue < 0 && StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer && !__instance.furniturePlacedAtQuotaStart.Contains(i))
                    __instance.furniturePlacedAtQuotaStart.Add(i);
            }
        }
    }
}
