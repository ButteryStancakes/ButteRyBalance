using HarmonyLib;

namespace ButteRyBalance.Patches.Enemies
{
    [HarmonyPatch(typeof(BushWolfEnemy))]
    class KidnapperFoxPatches
    {
        [HarmonyPatch(nameof(BushWolfEnemy.Start))]
        [HarmonyPostfix]
        static void BushWolfEnemy_Post_Start(BushWolfEnemy __instance)
        {
            if (__instance.IsOwner && Configuration.foxSquishy.Value)
                __instance.enemyHP = 6;
        }

        [HarmonyPatch(nameof(BushWolfEnemy.HitEnemy))]
        [HarmonyPrefix]
        static void BushWolfEnemy_Pre_HitEnemy(BushWolfEnemy __instance, ref int force)
        {
            // a bit easier to kill with both barrels at longer ranges
            if (force == 3 && __instance.IsOwner && Configuration.foxSquishy.Value)
                force++;
        }
    }
}
