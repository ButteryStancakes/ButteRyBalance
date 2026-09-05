using ButteRyBalance.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;

namespace ButteRyBalance.Utilities
{
    internal class ReflectionCache
    {
        internal static readonly FieldInfo TURBO_BOOSTS = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.turboBoosts)),
                                           BACK_DOOR_OPEN = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.backDoorOpen)),
                                           PHYSICS_PARENT = AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.physicsParent)),
                                           CAR_HP = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.carHP));
        internal static readonly MethodInfo IS_PLAYER_SAFE_IN_BACK = AccessTools.Method(typeof(VehicleControllerPatches), nameof(VehicleControllerPatches.IsPlayerSafeInBack)),
                                            NEXT = AccessTools.Method(typeof(System.Random), nameof(System.Random.Next), [typeof(int), typeof(int)]);
    }
}
