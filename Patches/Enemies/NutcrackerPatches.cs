using ButteRyBalance.Network;
using HarmonyLib;
using UnityEngine;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    class NutcrackerPatches
    {
        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.HitEnemy))]
        [HarmonyPrefix]
        static void NutcrackerEnemyAI_Pre_HitEnemy(NutcrackerEnemyAI __instance, ref int force, int hitID)
        {
            if (__instance.IsOwner && force == 5 && Configuration.nutcrackerKevlar.Value && (Common.DamageID)hitID == Common.DamageID.Unknown)
                force = 4;
        }

        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.GrabGun))]
        [HarmonyPostfix]
        static void NutcrackerEnemyAI_Post_GrabGun(NutcrackerEnemyAI __instance)
        {
            if (__instance.gun == null || !BRBNetworker.Instance.NutcrackerGunPrice.Value)
                return;

            if (Common.nutcrackerGuns.TryGetValue(__instance.gun, out int value))
            {
                if (__instance.gun.scrapValue == 60 && value != 60)
                {
                    __instance.gun.SetScrapValue(value);
                    RoundManager.Instance.totalScrapValueInLevel += value - 60;
                    Plugin.Logger.LogDebug($"Nutcracker #{__instance.NetworkObject.NetworkObjectId}: Gun #{__instance.gun.NetworkObject.NetworkObjectId} price $60 -> ${value}");
                }
            }
            else if (__instance.IsServer)
            {
                System.Random nutcrackerGunRandom = new(StartOfRound.Instance.randomMapSeed + 10000 + Common.nutcrackerGuns.Count);
                nutcrackerGunRandom.NextDouble();
                // 25 - v50 betas
                // 90 - max in v45
                // 100 - unused in "Shotgun" Item since v45
                BRBNetworker.Instance.SyncNutcrackerGunPriceRpc(__instance.gun.NetworkObject, nutcrackerGunRandom.Next(25, 90 + 1));
            }
        }
    }
}