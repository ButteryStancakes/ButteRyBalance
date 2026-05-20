using ButteRyBalance.Network;
using ButteRyBalance.Utilities;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    static class VehicleControllerPatches
    {
        const float MIN_STAMINA_DRAIN = 0.08f, MAX_STAMINA_DRAIN = 0.125f, STAMINA_MULT = 2f;

        internal static int criticalDurability = 16, turboBoosts = 5;
        internal static float regenInterval = 8f, scrapingStress = 0.2f, adjustableCrashSpeed = 28f;

        static float timeAtLastTreeDestroyed;

        [HarmonyPatch(nameof(VehicleController.Start))]
        [HarmonyPostfix]
        static void VehicleController_Post_Start(VehicleController __instance)
        {
            if (__instance.vehicleID == 0)
                Common.vehicleController = __instance;
        }

        [HarmonyPatch(nameof(VehicleController.CarReactToObstacle))]
        [HarmonyPrefix]
        static bool VehicleController_Pre_CarReactToObstacle(VehicleController __instance, ref int __state, CarObstacleType type, EnemyAI enemyScript, ref bool dealDamage)
        {
            __state = __instance.carHP;

            if (Common.INSTALLED_VERSION55_COMPANY_CRUISER || __instance.vehicleID != 0)
                return true;

            if (type == CarObstacleType.Enemy && enemyScript != null && BRBNetworker.Instance.FoxSlender.Value && enemyScript is BushWolfEnemy)
                return false;

            if (type == CarObstacleType.Object && Time.realtimeSinceStartup - timeAtLastTreeDestroyed < Time.fixedDeltaTime)
            {
                dealDamage = true;
                Plugin.Logger.LogDebug("Cruiser received damage in response to tree destruction");
            }

            return true;
        }

        [HarmonyPatch(nameof(VehicleController.CarReactToObstacle))]
        [HarmonyPostfix]
        static void VehicleController_Post_CarReactToObstacle(VehicleController __instance, int __state, Vector3 vel, CarObstacleType type, float obstacleSize, EnemyAI enemyScript)
        {
            if (Common.INSTALLED_VERSION55_COMPANY_CRUISER || __instance.vehicleID != 0 || __instance.carHP <= 0)
                return;

            if (type == CarObstacleType.Enemy && enemyScript != null && __instance.carHP <= __state && BRBNetworker.Instance.CruiserEnemyDamage.Value)
            {
                // enemy collisions deal 2 damage
                int damageLeftToDeal = __instance.carHP - (__state - 2);
                if (damageLeftToDeal < 1)
                    return;

                // no extra damage if car is parked with the engine off, and no players are nearby
                if (!__instance.ignitionStarted && __instance.gear == CarGearShift.Park && __instance.currentDriver == null && __instance.currentPassenger == null)
                {
                    bool nobodyNearCar = true;
                    for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                    {
                        if (StartOfRound.Instance.allPlayerScripts[i] == null)
                            continue;

                        if (StartOfRound.Instance.allPlayerScripts[i].physicsParent != null && StartOfRound.Instance.allPlayerScripts[i].physicsParent == __instance.transform)
                        {
                            nobodyNearCar = false;
                            break;
                        }
                    }

                    if (nobodyNearCar)
                        return;
                }

                // tulip snakes jostling the car is fine, but they shouldn't deal damage
                if (enemyScript is FlowerSnakeEnemy)
                    return;

                float forgivenessVelocity = obstacleSize > 2f ? 15f : 9f;
                Vector3 velocity = vel;
                velocity.y = 0f;
                if (velocity.magnitude >= forgivenessVelocity)
                    return;

                Plugin.Logger.LogDebug($"Cruiser takes extra {damageLeftToDeal} damage from collision with enemy \"{enemyScript.name}\" (#{enemyScript.GetInstanceID()})");
                __instance.DealPermanentDamage(damageLeftToDeal);
            }
        }

        [HarmonyPatch(nameof(VehicleController.DoTurboBoost))]
        [HarmonyPrefix]
        static bool VehicleController_Pre_DoTurboBoost(VehicleController __instance, InputAction.CallbackContext context)
        {
            if (Common.INSTALLED_VERSION55_COMPANY_CRUISER || __instance.vehicleID != 0 || __instance.turboBoosts > 0 || !BRBNetworker.Instance.CruiserExhaust.Value)
                return true;

            if (context.performed && __instance.localPlayerInControl && !__instance.keyIsInDriverHand && __instance.currentDriver == GameNetworkManager.Instance.localPlayerController)
            {
                if (__instance.currentDriver.isExhausted || __instance.currentDriver.sprintMeter <= 0.3f)
                    return false;

                if (!__instance.jumpingInCar)
                {
                    float staminaDrain = MIN_STAMINA_DRAIN;
                    if (Vector3.Angle(Vector3.up, __instance.transform.forward) < Vector3.Angle(Vector3.down, __instance.transform.forward))
                        staminaDrain = Mathf.Lerp(staminaDrain, MAX_STAMINA_DRAIN, (Vector3.Angle(Vector3.up, __instance.transform.up) - __instance.physicsRegion.maxTippingAngle) / (90 - __instance.physicsRegion.maxTippingAngle));

                    __instance.currentDriver.sprintMeter = Mathf.Clamp01(__instance.currentDriver.sprintMeter - (staminaDrain * STAMINA_MULT));
                    if (__instance.currentDriver.sprintMeter <= 0.3f)
                    {
                        __instance.currentDriver.sprintMeter = 0.1f;
                        __instance.currentDriver.isExhausted = true;
                    }
                }

                return true;
            }

            return true;
        }

        [HarmonyPatch(nameof(VehicleController.ReactToDamage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> VehicleController_Trans_ReactToDamage(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            FieldInfo carHP = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.carHP)),
                      timeAtLastDamage = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.timeAtLastDamage));
            bool patchHP = false, patchTime = false, patchTurbo = false;
            for (int i = 2; i < codes.Count; i++)
            {
                if (!patchHP && codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == criticalDurability && codes[i - 1].opcode == OpCodes.Ldfld && (FieldInfo)codes[i - 1].operand == carHP)
                {
                    codes[i].opcode = OpCodes.Ldsfld;
                    codes[i].operand = AccessTools.Field(typeof(VehicleControllerPatches), nameof(criticalDurability));
                    patchHP = true;
                }
                else if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i - 2].opcode == OpCodes.Ldfld)
                {
                    float operand = (float)codes[i].operand;
                    FieldInfo field = (FieldInfo)codes[i - 2].operand;
                    if (!patchTime && operand == regenInterval && codes[i - 1].opcode == OpCodes.Sub && field == timeAtLastDamage)
                    {
                        codes[i].opcode = OpCodes.Ldsfld;
                        codes[i].operand = AccessTools.Field(typeof(VehicleControllerPatches), nameof(regenInterval));
                        patchTime = true;
                    }
                    else if (!patchTurbo && operand == 5f && field == ReflectionCache.TURBO_BOOSTS)
                    {
                        codes[i].opcode = OpCodes.Call;
                        codes[i].operand = AccessTools.Method(typeof(VehicleControllerPatches), nameof(GetTurboDivisor));
                        patchTurbo = true;
                    }
                }

                if (patchHP && patchTime && patchTurbo)
                {
                    Plugin.Logger.LogDebug($"Transpiler (Cruiser): Dynamic regen/meter");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning($"Cruiser regen transpiler failed");
            return instructions;
        }

        internal static void JustDestroyedTree()
        {
            Plugin.Logger.LogDebug("A tree was just destroyed");
            if (Common.vehicleController != null && Common.vehicleController.vehicleID == 0 && !Common.INSTALLED_VERSION55_COMPANY_CRUISER && Common.vehicleController.IsOwner && BRBNetworker.Instance.CruiserTrees.Value)
                timeAtLastTreeDestroyed = Time.realtimeSinceStartup;
        }

        internal static void DealExtraCrashDamage(VehicleController vehicleController, int damage)
        {
            if (vehicleController != null && vehicleController.vehicleID == 0 && !Common.INSTALLED_VERSION55_COMPANY_CRUISER && BRBNetworker.Instance.CruiserCrashDamage.Value)
            {
                Plugin.Logger.LogDebug($"Cruiser takes extra {damage} damage from collision with geometry");
                vehicleController.DealPermanentDamage(damage);
            }
        }

        [HarmonyPatch(nameof(VehicleController.OnCollisionEnter))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> VehicleController_Trans_OnCollisionEnter(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            MethodInfo setInternalStress = AccessTools.Method(typeof(VehicleController), nameof(VehicleController.SetInternalStress)),
                       magnitude = AccessTools.DeclaredPropertyGetter(typeof(Vector3), nameof(Vector3.magnitude)),
                       dealPermanentDamage = AccessTools.Method(typeof(VehicleController), nameof(VehicleController.DealPermanentDamage)),
                       dealExtraCrashDamage = AccessTools.Method(typeof(VehicleControllerPatches), nameof(DealExtraCrashDamage));
            FieldInfo averageVelocity = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.averageVelocity));
            bool patchStress = false, patchVelocity = false;
            int damageCalls = 0;
            for (int i = 4; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4)
                {
                    if (!patchStress && (float)codes[i].operand == scrapingStress && codes[i + 1].opcode == OpCodes.Call && codes[i + 1].operand as MethodInfo == setInternalStress)
                    {
                        codes[i].opcode = OpCodes.Ldsfld;
                        codes[i].operand = AccessTools.Field(typeof(VehicleControllerPatches), nameof(scrapingStress));
                        patchStress = true;
                    }
                    else if (!patchVelocity && (float)codes[i].operand == adjustableCrashSpeed && codes[i - 1].opcode == OpCodes.Call && codes[i - 1].operand as MethodInfo == magnitude)
                    {
                        codes[i].opcode = OpCodes.Ldsfld;
                        codes[i].operand = AccessTools.Field(typeof(VehicleControllerPatches), nameof(adjustableCrashSpeed));
                        patchVelocity = true;
                    }
                }
                else if (damageCalls == 0 && patchVelocity && codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == dealPermanentDamage && codes[i - 4].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.InsertRange(i + 1, [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldc_I4_4),
                        new(OpCodes.Call, dealExtraCrashDamage)]);
                    i += 3;
                    damageCalls++;
                }
                else if (damageCalls == 1 && codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == dealPermanentDamage && codes[i - 4].opcode == OpCodes.Ldc_I4_1)
                {
                    codes.InsertRange(i + 1, [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldc_I4_1),
                        new(OpCodes.Call, dealExtraCrashDamage)]);
                    i += 3;
                    damageCalls++;
                }
                else if (damageCalls == 2 && codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == magnitude && codes[i - 1].opcode == OpCodes.Ldflda && (FieldInfo)codes[i - 1].operand == averageVelocity && codes[i + 1].opcode == OpCodes.Ldc_R4 && (float)codes[i + 1].operand == 1.5f && codes[i - 3].opcode == OpCodes.Br)
                {
                    codes.InsertRange(i - 3, [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldc_I4_1),
                        new(OpCodes.Call, dealExtraCrashDamage)]);
                    i += 3;
                    damageCalls++;
                }

                if (patchStress && patchVelocity && damageCalls == 3)
                {
                    Plugin.Logger.LogDebug($"Transpiler (Cruiser): Extra crash damage");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning($"Cruiser crash transpiler failed");
            return instructions;
        }

        public static bool IsPlayerSafeInBack(VehicleController vehicleController, PlayerControllerB player)
        {
            if (vehicleController == null)
                return false;

            if (player == null || Common.INSTALLED_VERSION55_COMPANY_CRUISER || vehicleController.vehicleID != 0 || BRBNetworker.Instance == null || !BRBNetworker.Instance.CruiserPatchEnemies.Value)
                return !vehicleController.backDoorOpen;

            if (vehicleController.backDoorOpen)
                return false;

            // player is in one of the seats
            if (player.inVehicleAnimation || vehicleController.currentDriver == player || vehicleController.currentPassenger == player)
                return false;

            // player is on top of the Cruiser
            if (vehicleController.ontopOfTruckCollider.bounds.ClosestPoint(player.transform.position) == player.transform.position)
                return false;

            return true;
        }

        [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.OnCollideWithPlayer))]
        [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.OnCollideWithPlayer))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> EnemyAI_Trans_OnCollideWithPlayer(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 8; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && (FieldInfo)codes[i].operand == ReflectionCache.BACK_DOOR_OPEN && codes[i + 1].opcode == OpCodes.Ldc_I4_0 && codes[i + 2].opcode == OpCodes.Ceq && codes[i - 6].opcode == OpCodes.Ldfld && (FieldInfo)codes[i - 6].operand == ReflectionCache.PHYSICS_PARENT)
                {
                    codes[i + 1].opcode = OpCodes.Ldc_I4_1;
                    codes[i] = new(OpCodes.Call, ReflectionCache.IS_PLAYER_SAFE_IN_BACK);
                    codes.Insert(i, new(codes[i - 7].opcode, codes[i - 7].operand));
                    //i++;
                    Plugin.Logger.LogDebug($"Transpiler ({__originalMethod.DeclaringType}.{__originalMethod.Name}): Patch backdoor protection");
                    return codes;
                }
            }

            return instructions;
        }

        public static float GetTurboDivisor()
        {
            return Mathf.Max(turboBoosts, 1f);
        }

        [HarmonyPatch(nameof(VehicleController.AddTurboBoost))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> VehicleController_Trans_AddTurboBoost(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 3; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_5 && codes[i - 3].opcode == OpCodes.Ldfld && (FieldInfo)codes[i - 3].operand == ReflectionCache.TURBO_BOOSTS)
                {
                    codes[i].opcode = OpCodes.Ldsfld;
                    codes[i].operand = AccessTools.Field(typeof(VehicleControllerPatches), nameof(turboBoosts));
                    Plugin.Logger.LogDebug($"Transpiler (Cruiser): Dynamic turbo");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning($"Cruiser turbo transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(VehicleController.AddTurboBoost))]
        [HarmonyPrefix]
        static bool VehicleController_Pre_AddTurboBoost()
        {
            return turboBoosts > 0;
        }
    }
}
