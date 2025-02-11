using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    class TimeOfDayPatches
    {
        static bool rollbackFog;

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

            if (StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.Foggy || !__instance.foggyWeather.enabled || !BRBNetworker.Instance.FoggyRework.Value)
                return;

            switch (StartOfRound.Instance.currentLevel.sceneName)
            {
                // fog patch applies to all of these maps
                case "Level1Experimentation":
                case "Level2Assurance":
                case "Level3Vow":
                case "Level4March":
                case "Level7Offense":
                case "Level10Adamance":
                case "Level11Embrion":
                    LocalVolumetricFog localVolumetricFog = GameObject.Find(fogPath)?.GetComponent<LocalVolumetricFog>();
                    if (localVolumetricFog != null)
                    {
                        localVolumetricFog.parameters.meanFreePath = Mathf.Max(__instance.foggyWeather.parameters.meanFreePath, 7f);
                        Plugin.Logger.LogDebug($"{StartOfRound.Instance.currentLevel.sceneName} Foggy: {localVolumetricFog.parameters.meanFreePath}");
                        __instance.foggyWeather.enabled = false;
                        rollbackFog = true;
                    }
                    break;
            }
        }

        [HarmonyPatch(nameof(TimeOfDay.OnDayChanged))]
        [HarmonyPostfix]
        static void TimeOfDay_Post_OnDayChanged(TimeOfDay __instance)
        {
            if (rollbackFog)
            {
                __instance.foggyWeather.enabled = true;
                rollbackFog = false;
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
