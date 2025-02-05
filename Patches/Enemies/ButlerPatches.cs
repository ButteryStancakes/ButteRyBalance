using ButteRyBalance.Network;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(ButlerEnemyAI))]
    class ButlerPatches
    {
        [HarmonyPatch(nameof(ButlerEnemyAI.OnCollideWithPlayer))]
        [HarmonyPrefix]
        static bool ButlerEnemyAI_Pre_OnCollideWithPlayer(ButlerEnemyAI __instance, Collider other)
        {
            // should custom logic run?
            if (!BRBNetworker.Instance.ButlerStealthStab.Value && !BRBNetworker.Instance.ButlerLongCooldown.Value)
                return true;

            if (__instance.isEnemyDead)
                return false;

            if (__instance.currentBehaviourStateIndex != 2 && __instance.berserkModeTimer <= 0f)
            {
                // only run once every 10s
                if (Time.realtimeSinceStartup - __instance.timeSinceStealthStab < 10f)
                    return false;
                __instance.timeSinceStealthStab = Time.realtimeSinceStartup;

                // 95% chance to ignore
                if (Random.Range(0, 100) < 95)
                    return false;
            }

            // limit attack rate
            float cooldown = 0.25f;
            if (BRBNetworker.Instance.ButlerLongCooldown.Value && __instance.timeAtLastButlerDamage <= 0f && __instance.berserkModeTimer <= 3f)
                cooldown = 0.35f;
            if (__instance.timeSinceHittingPlayer < cooldown)
                return false;

            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player != null)
            {
                __instance.timeSinceHittingPlayer = 0f;
                player.DamagePlayer(10, causeOfDeath: CauseOfDeath.Stabbing);

                bool berserk = __instance.currentBehaviourStateIndex != 2;
                if (berserk && BRBNetworker.Instance.ButlerStealthStab.Value)
                {
                    // are any players (other than the one being stabbed) in vicinity?
                    berserk = !player.NearOtherPlayers(player, 15f);
                    if (berserk)
                    {
                        for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                        {
                            if (StartOfRound.Instance.allPlayerScripts[i] == GameNetworkManager.Instance.localPlayerController)
                                continue;

                            if (!StartOfRound.Instance.allPlayerScripts[i].isPlayerDead && StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled && __instance.CheckLineOfSightForPosition(StartOfRound.Instance.allPlayerScripts[i].gameplayCamera.transform.position, 110f, proximityAwareness: 2f))
                            {
                                berserk = false;
                                break;
                            }
                        }
                    }
                }

                if (berserk)
                    __instance.berserkModeTimer = 3f;
                __instance.StabPlayerServerRpc((int)player.playerClientId, berserk);
            }

            return false;
        }

        [HarmonyPatch(nameof(ButlerEnemyAI.HitEnemy))]
        [HarmonyPostfix]
        static void ButlerEnemyAI_Post_HitEnemy(ButlerEnemyAI __instance, PlayerControllerB playerWhoHit)
        {
            if (BRBNetworker.Instance.ButlerLongCooldown.Value && playerWhoHit != null)
                __instance.timeAtLastButlerDamage = Time.realtimeSinceStartup;
        }

        [HarmonyPatch(nameof(ButlerEnemyAI.Start))]
        [HarmonyPostfix]
        static void ButlerEnemyAI_Post_Start(ButlerEnemyAI __instance)
        {
            if (BRBNetworker.Instance.ButlerLongCooldown.Value)
                __instance.enemyHP = Mathf.Max(__instance.enemyHP, 3);
        }
    }
}
