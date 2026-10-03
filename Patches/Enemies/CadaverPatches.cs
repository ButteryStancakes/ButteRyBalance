using ButteRyBalance.Utilities;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.VFX;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch]
    static class CadaverPatches
    {
        static float timeOfDaySpawned;

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
            /*if (Configuration.cadaversLimitGrowth.Value)
                __instance.GrowthChancePerInterval = 20f;*/

            timeOfDaySpawned = TimeOfDay.Instance.normalizedTimeOfDay;
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

        public static float GetCadaverGrowthTime()
        {
            if (Configuration.cadaversLimitGrowth.Value)
                return Mathf.InverseLerp(timeOfDaySpawned, 1f, TimeOfDay.Instance.normalizedTimeOfDay);

            return TimeOfDay.Instance.normalizedTimeOfDay;
        }

        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.DoAIInterval))]
        [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.LateUpdate))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CadaverGrowthAI_Trans(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            List<CodeInstruction> codes = instructions.ToList();

            bool done = false;
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == ReflectionCache.TIME_OF_DAY_INSTANCE && codes[i + 1].opcode == OpCodes.Ldfld && (FieldInfo)codes[i + 1].operand == ReflectionCache.NORMALIZED_TIME_OF_DAY && codes[i + 2].opcode == OpCodes.Callvirt && codes[i + 2].operand as MethodInfo == ReflectionCache.EVALUATE)
                {
                    codes[i].operand = ReflectionCache.GET_CADAVER_GROWTH_TIME;
                    codes.RemoveAt(i + 1);
                    Plugin.Logger.LogDebug($"Transpiler ({__originalMethod.DeclaringType}.{__originalMethod.Name}): GetCadaverGrowthTime()");
                    done = true;
                }
            }

            if (!done)
                Plugin.Logger.LogError($"{__originalMethod.DeclaringType}.{__originalMethod.Name} transpiler failed");

            return codes;
        }
    }
}
