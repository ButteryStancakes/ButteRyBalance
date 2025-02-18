using ButteRyBalance.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance
{
    internal class Common
    {
        // Experimentation, Assurance, Vow, Gordion, March, Adamance, Rend, Dine, Offense, Titan, Artifice, Liquidation, Embrion
        internal const int NUM_LEVELS = 13;

        internal static bool INSTALLED_ARTIFICE_BLIZZARD, INSTALLED_BARBER_FIXES, INSTALLED_VENT_SPAWN_FIX;
        internal static GameObject artificeBlizzard;

        internal static Dictionary<string, EnemyType> enemies = [];

        internal static void Disconnect()
        {
            enemies.Clear();
            StartOfRoundPatches.skipWeatherPatch = true;
        }

        internal static bool IsSnowLevel()
        {
            return StartOfRound.Instance.currentLevel.levelIncludesSnowFootprints && (artificeBlizzard == null || artificeBlizzard.activeSelf);
        }
    }
}
