using ButteRyBalance;
using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ButteryBalance.Patches.Objects
{
    [HarmonyPatch(typeof(Turret))]
    static class TurretPatches
    {
        [HarmonyPatch(nameof(Turret.CheckForPlayersInLineOfSight))]
        [HarmonyPostfix]
        static void Turret_Post_CheckForPlayersInLineOfSight(Turret __instance, ref PlayerControllerB __result)
        {
            if (__result != null && __instance.turretMode != TurretMode.Firing && __instance.turretMode != TurretMode.Berserk && RoundManager.Instance.indoorFog.gameObject.activeSelf && BRBNetworker.Instance.IndoorFogSight.Value && Vector3.Distance(__instance.hit.point, __instance.centerPoint.position) > Common.INTERIOR_FOG_RANGE)
                __result = null;
        }
    }
}
