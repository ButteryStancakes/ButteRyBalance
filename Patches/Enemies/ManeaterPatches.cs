using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(CaveDwellerAI))]
    class ManeaterPatches
    {
        static bool playersHaveEnteredBuilding;

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
            if (setCrying && !__instance.isOutside && __instance.currentBehaviourStateIndex == 0 && !playersHaveEnteredBuilding && BRBNetworker.Instance.ManeaterLimitGrowth.Value)
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

        [HarmonyPatch(nameof(CaveDwellerAI.DoNonBabyUpdateLogic))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_DoNonBabyUpdateLogic(CaveDwellerAI __instance)
        {
            if (__instance.IsOwner && BRBNetworker.Instance.ManeaterWideTurns.Value)
            {
                if (__instance.leaping && !__instance.isOutside)
                {
                    __instance.agent.angularSpeed = 220f; //320
                    //__instance.agent.acceleration = 75f;
                }
                else
                {
                    __instance.agent.angularSpeed = 700f;
                    //__instance.agent.acceleration = 425f;
                }
            }
        }

        [HarmonyPatch(nameof(CaveDwellerAI.StartTransformationAnim))]
        [HarmonyPostfix]
        static void CaveDwellerAI_Post_StartTransformationAnim(CaveDwellerAI __instance)
        {
            if (BRBNetworker.Instance.ManeaterTarget.Value)
            {
                __instance.gameObject.layer = LayerMask.NameToLayer("Enemies");

                if (!__instance.GetComponents<Collider>().Any(collider => collider.isTrigger))
                {
                    float scalar = 1f / ((__instance.transform.localScale.x + __instance.transform.localScale.y + __instance.transform.localScale.z) / 3f);

                    BoxCollider boxCollider = __instance.gameObject.AddComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                    boxCollider.center = new(0f, 2.13f * scalar, 0f);
                    boxCollider.size = new Vector3(scalar, scalar, scalar);
                }
            }
        }

        [HarmonyPatch(nameof(CaveDwellerAI.BabyObserveScrap))]
        [HarmonyPrefix]
        static bool CaveDwellerAI_Pre_BabyObserveScrap(CaveDwellerAI __instance, ref bool __result)
        {
            if (__instance.scrapEaten >= 3f)
            {
                __result = false; // baby refuses to eat scrap items at max chunkiness
                return false;
            }

            return true;
        }

        [HarmonyPatch(nameof(CaveDwellerAI.ClearBabyObservingClientRpc))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CaveDwellerAI_Trans_ClearBabyObservingClientRpc(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 4; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Div && codes[i - 1].opcode == OpCodes.Ldc_R4 && (float)codes[i - 1].operand == 5f && codes[i - 4].opcode == OpCodes.Ldc_R4 && (float)codes[i - 4].operand == 5f)
                {
                    codes[i - 1].operand = 3f;
                    codes[i - 4].operand = 3f;
                    Plugin.Logger.LogDebug($"Transpiler (Maneater): Max scale after eating 3 items");
                    return codes;
                }
            }

            return instructions;
        }
    }
}
