using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(CaveDwellerAI))]
    class ManeaterPatches
    {
        static bool playersHaveEnteredBuilding;
        static float GROWTH_SPEED_MULTIPLIER = 0.045f;

        [HarmonyPatch(nameof(CaveDwellerAI.HitEnemy))]
        [HarmonyPrefix]
        static void CaveDwellerAI_Pre_HitEnemy(CaveDwellerAI __instance, ref float __state)
        {
            __state = __instance.growthMeter;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.HitEnemy))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_HitEnemy(CaveDwellerAI __instance, float __state, PlayerControllerB playerWhoHit)
        {
            if (__instance.IsServer && __instance.currentBehaviourStateIndex == 0 && playerWhoHit == null && !__instance.hasPlayerFoundBaby && BRBNetworker.Instance.ManeaterLimitGrowth.Value)
                __instance.growthMeter = __state;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.KillEnemy))]
        [HarmonyPrefix]
        static void CaveDwellerAI_Pre_KillEnemy(CaveDwellerAI __instance, ref float __state)
        {
            __state = __instance.growthMeter;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.KillEnemy))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_KillEnemy(CaveDwellerAI __instance, float __state)
        {
            if (__instance.IsServer && __instance.currentBehaviourStateIndex == 0 && !__instance.hasPlayerFoundBaby && BRBNetworker.Instance.ManeaterLimitGrowth.Value)
                __instance.growthMeter = __state;
        }

        [HarmonyPatch(typeof(EntranceTeleport), nameof(EntranceTeleport.TeleportPlayerClientRpc))]
        [HarmonyPostfix]
        static void EntranceTeleport_Post_TeleportPlayerClientRpc()
        {
            playersHaveEnteredBuilding = true;
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.EndOfGameClientRpc))]
        [HarmonyPostfix]
        static void StartOfRound_Post_EndOfGameClientRpc()
        {
            playersHaveEnteredBuilding = false;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.SetCryingLocalClient))]
        [HarmonyPrefix]
        static void CaveDwellerAI_Pre_SetCryingLocalClient(CaveDwellerAI __instance, ref bool setCrying)
        {
            if (__instance.currentBehaviourStateIndex == 0 && !playersHaveEnteredBuilding && BRBNetworker.Instance.ManeaterLimitGrowth.Value)
                setCrying = false;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.IncreaseBabyGrowthMeter))]
        [HarmonyPrefix]
        static void CaveDwellerAI_Pre_IncreaseBabyGrowthMeter(CaveDwellerAI __instance, ref float __state)
        {
            __state = __instance.growthMeter;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.IncreaseBabyGrowthMeter))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_IncreaseBabyGrowthMeter(CaveDwellerAI __instance, float __state)
        {
            if (BRBNetworker.Instance.ManeaterLimitGrowth.Value)
            {
                if (!playersHaveEnteredBuilding)
                    __instance.growthMeter = 0f;
                // grow slower when held, if not being shaken
                else if (__instance.propScript?.playerHeldBy != null && __instance.rockingBaby < 2 && !__instance.stopCryingWhenReleased)
                    __instance.growthMeter = __state + ((__instance.growthMeter - __state) / 1.3f);
            }
        }

        [HarmonyPatch(nameof(CaveDwellerAI.Start))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_Start(CaveDwellerAI __instance)
        {
            GROWTH_SPEED_MULTIPLIER = __instance.growthSpeedMultiplier;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.StopObserving))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_StopObserving(CaveDwellerAI __instance, bool eatScrap)
        {
            if (eatScrap && Configuration.maneaterScrapGrowth.Value)
            {
                if (__instance.scrapEaten == 1)
                    __instance.growthSpeedMultiplier = GROWTH_SPEED_MULTIPLIER * 0.75f;
                else
                    __instance.growthSpeedMultiplier = GROWTH_SPEED_MULTIPLIER * 0.5f;
            }
        }

        [HarmonyPatch(nameof(CaveDwellerAI.DoNonBabyUpdateLogic))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_DoNonBabyUpdateLogic(CaveDwellerAI __instance)
        {
            if (__instance.IsOwner && BRBNetworker.Instance.ManeaterWideTurns.Value)
            {
                if (__instance.leaping && !__instance.isOutside)
                {
                    __instance.agent.angularSpeed = 320f;
                    __instance.agent.acceleration = 75f;
                }
                else
                {
                    __instance.agent.angularSpeed = 700f;
                    __instance.agent.acceleration = 425f;
                }
            }
        }
    }
}
