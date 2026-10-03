using ButteRyBalance.Patches;
using ButteRyBalance.Patches.Enemies;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace ButteRyBalance.Utilities
{
    internal class ReflectionCache
    {
        internal static readonly FieldInfo TURBO_BOOSTS = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.turboBoosts)),
                                           BACK_DOOR_OPEN = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.backDoorOpen)),
                                           PHYSICS_PARENT = AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.physicsParent)),
                                           CAR_HP = AccessTools.Field(typeof(VehicleController), nameof(VehicleController.carHP)),

                                           MAX_VALUE = AccessTools.Field(typeof(Item), nameof(Item.maxValue)),

                                           NORMALIZED_TIME_OF_DAY = AccessTools.Field(typeof(TimeOfDay), nameof(TimeOfDay.normalizedTimeOfDay)),

                                           IS_ENEMY_DEAD = AccessTools.Field(typeof(EnemyAI), nameof(EnemyAI.isEnemyDead)),
                                           ENEMY_TYPE = AccessTools.Field(typeof(EnemyAI), nameof(EnemyAI.enemyType)),
                                           CAN_DIE = AccessTools.Field(typeof(EnemyType), nameof(EnemyType.canDie));

        internal static readonly MethodInfo IS_PLAYER_SAFE_IN_BACK = AccessTools.Method(typeof(VehicleControllerPatches), nameof(VehicleControllerPatches.IsPlayerSafeInBack)),

                                            NEXT = AccessTools.Method(typeof(System.Random), nameof(System.Random.Next), [typeof(int), typeof(int)]),

                                            GET_CADAVER_GROWTH_TIME = AccessTools.Method(typeof(CadaverPatches), nameof(CadaverPatches.GetCadaverGrowthTime)),
                                            TIME_OF_DAY_INSTANCE = AccessTools.DeclaredPropertyGetter(typeof(TimeOfDay), nameof(TimeOfDay.Instance)),
                                            EVALUATE = AccessTools.Method(typeof(AnimationCurve), nameof(AnimationCurve.Evaluate)),

                                            HIT_ENEMY = AccessTools.Method(typeof(EnemyAI), nameof(EnemyAI.HitEnemy)),
                                            HIT_ENEMY_ON_LOCAL_CLIENT = AccessTools.Method(typeof(EnemyAI), nameof(EnemyAI.HitEnemyOnLocalClient)),
                                            KILL_ENEMY = AccessTools.Method(typeof(EnemyAI), nameof(EnemyAI.KillEnemy)),
                                            KILL_ENEMY_ON_OWNER_CLIENT = AccessTools.Method(typeof(EnemyAI), nameof(EnemyAI.KillEnemyOnOwnerClient)),

                                            IS_OWNER = AccessTools.DeclaredPropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.IsOwner)),

                                            ZERO = AccessTools.DeclaredPropertyGetter(typeof(Vector3), nameof(Vector3.zero));
    }
}
