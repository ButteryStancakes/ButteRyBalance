using ButteRyBalance.Network;
using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    class NutcrackerPatches
    {
        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.GrabGun))]
        [HarmonyPostfix]
        static void NutcrackerEnemyAI_Post_GrabGun(NutcrackerEnemyAI __instance)
        {
            if (!__instance.IsServer && __instance.gun.scrapValue == 60 && BRBNetworker.Instance.NutcrackerGunPrice.Value)
                RandomizeShotgunPrice(__instance.gun, __instance.randomSeedNumber);
        }

        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.InitializeNutcrackerValuesClientRpc))]
        [HarmonyPostfix]
        static void NutcrackerEnemyAI_Post_InitializeNutcrackerValuesClientRpc(NutcrackerEnemyAI __instance)
        {
            if (__instance.gun != null && BRBNetworker.Instance.NutcrackerGunPrice.Value)
                RandomizeShotgunPrice(__instance.gun, __instance.randomSeedNumber);
        }

        static void RandomizeShotgunPrice(ShotgunItem gun, int seed)
        {
            // 25 - v50 betas
            // 90 - max in v45
            // 100 - unused in "Shotgun" Item since v45
            gun.SetScrapValue(new System.Random(seed).Next(25, 90));
        }

        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.HitEnemy))]
        [HarmonyPrefix]
        static void NutcrackerEnemyAI_Pre_HitEnemy(NutcrackerEnemyAI __instance, ref int force)
        {
            if (__instance.IsOwner && force == 5 && Configuration.nutcrackerKevlar.Value)
                force = 4;
        }
    }
}
