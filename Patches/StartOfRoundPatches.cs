using ButteRyBalance.Network;
using ButteRyBalance.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    static class StartOfRoundPatches
    {
        static float moldChance1 = 0.03f, moldChance2 = 0.045f, moldChance3 = 0.015f, moldChance4 = 0.022f;

        [HarmonyPatch(nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        static void StartOfRound_Post_Awake(StartOfRound __instance)
        {
            // cache references to all vanilla enemies
            if (Common.enemies.Count < 1)
            {
                SelectableLevel testAllEnemiesLevel = Object.FindAnyObjectByType<QuickMenuManager>()?.testAllEnemiesLevel;
                if (testAllEnemiesLevel != null)
                {
                    foreach (List<SpawnableEnemyWithRarity> enemyList in new List<SpawnableEnemyWithRarity>[]{
                        testAllEnemiesLevel.Enemies,
                        testAllEnemiesLevel.OutsideEnemies,
                        testAllEnemiesLevel.DaytimeEnemies
                    })
                    {
                        foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in enemyList)
                        {
                            if (!Common.enemies.ContainsKey(spawnableEnemyWithRarity.enemyType.name))
                                Common.enemies.Add(spawnableEnemyWithRarity.enemyType.name, spawnableEnemyWithRarity.enemyType);
                        }
                    }
                }
            }

            // need to add enemies to indoors on all clients so vent sounds and infestations will sync
            EnemyType masked = Common.enemies["MaskedPlayerEnemy"], butler = Common.enemies["Butler"];
            if (masked != null && butler != null)
            {
                foreach (SelectableLevel level in __instance.levels)
                {
                    if (masked != null && (level.sceneName == "Level2Assurance" /*|| level.sceneName == "Level6Dine"*/ || level.sceneName == "Level7Offense") && !level.Enemies.Any(enemy => enemy.enemyType == masked))
                    {
                        level.Enemies.Add(new(masked, 0));
                        Plugin.Logger.LogDebug($"Added Masked to {level.name} pool on client (fallback)");
                    }
                    else if (butler != null && level.sceneName == "Level10Adamance" && !level.Enemies.Any(enemy => enemy.enemyType == butler))
                    {
                        level.Enemies.Add(new(butler, 0));
                        Plugin.Logger.LogDebug($"Added butlers to {level.name} pool on client (fallback)");
                    }
                }
            }
            else
                Plugin.Logger.LogWarning("Failed to reference \"masked\" or butler enemy types. This should never happen");

            BRBNetworker.Create();
        }

        [HarmonyPatch(nameof(StartOfRound.PassTimeToNextDay))]
        [HarmonyPostfix]
        static void StartOfRound_Post_PassTimeToNextDay()
        {
            RoundManager.Instance.hasInitializedLevelRandomSeed = false;
        }

        [HarmonyPatch(nameof(StartOfRound.SetPlanetsMold))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> StartOfRound_Trans_SetPlanetsMold(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            bool patch1 = false, patch2 = false, patch3 = false, patch4 = false;
            for (int i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_1 && codes[i - 1].opcode == OpCodes.Ldc_R4)
                {
                    if (!patch1 && (float)codes[i - 1].operand == moldChance1)
                    {
                        codes[i - 1].opcode = OpCodes.Ldsfld;
                        codes[i - 1].operand = AccessTools.Field(typeof(StartOfRoundPatches), nameof(moldChance1));
                        patch1 = true;
                    }
                    else if (!patch2 && (float)codes[i - 1].operand == moldChance2)
                    {
                        codes[i - 1].opcode = OpCodes.Ldsfld;
                        codes[i - 1].operand = AccessTools.Field(typeof(StartOfRoundPatches), nameof(moldChance2));
                        patch2 = true;
                    }
                    else if (!patch3 && (float)codes[i - 1].operand == moldChance3)
                    {
                        codes[i - 1].opcode = OpCodes.Ldsfld;
                        codes[i - 1].operand = AccessTools.Field(typeof(StartOfRoundPatches), nameof(moldChance3));
                        patch3 = true;
                    }
                    else if (!patch4 && (float)codes[i - 1].operand == moldChance4)
                    {
                        codes[i - 1].opcode = OpCodes.Ldsfld;
                        codes[i - 1].operand = AccessTools.Field(typeof(StartOfRoundPatches), nameof(moldChance4));
                        patch4 = true;
                    }
                }

                if (patch1 && patch2 && patch3 && patch4)
                {
                    Plugin.Logger.LogDebug($"Transpiler (Vain shrouds): Dynamic chances");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning("Vain shroud chance transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(StartOfRound.SetPlanetsMold))]
        [HarmonyPrefix]
        static void StartOfRound_Pre_SetPlanetsMold(StartOfRound __instance)
        {
            if (__instance.IsServer)
            {
                moldChance1 = Configuration.vainsChanceSameEarly.Value / 100f;
                moldChance2 = Configuration.vainsChanceSame.Value / 100f;
                moldChance3 = Configuration.vainsChanceRare.Value / 100f;
                moldChance4 = Configuration.vainsChanceOther.Value / 100f;
                Plugin.Logger.LogDebug($"Adjusted vain shroud growth chances on host: ({moldChance1 * 100}%, {moldChance2 * 100}%, {moldChance3 * 100}%, {moldChance4 * 100}%)");
            }
        }
    }
}
