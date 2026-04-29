using ButteRyBalance.Patches;
using DunGen;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance
{
    internal class Common
    {
        internal enum DamageID
        {
            Unknown = -1,
            Shovel = 1,
            Knife = 5,
            Cruiser = 331
        }

        // Experimentation, Assurance, Vow, Gordion, March, Adamance, Rend, Dine, Offense, Titan, Artifice, Liquidation, Embrion
        internal const int NUM_LEVELS = 13;

        internal static bool INSTALLED_ARTIFICE_BLIZZARD, INSTALLED_BARBER_FIXES, INSTALLED_SPAWN_CYCLE_FIXES;
        internal static GameObject artificeBlizzard;

        internal static Dictionary<string, EnemyType> enemies = [];

        static Terminal terminal;
        internal static Terminal Terminal
        {
            get
            {
                if (terminal == null)
                    terminal = Object.FindAnyObjectByType<Terminal>();

                return terminal;
            }
        }

        internal static List<Bounds> caveTiles = [];

        internal static void Disconnect()
        {
            enemies.Clear();
            caveTiles.Clear();
        }

        internal static bool IsSnowLevel()
        {
            return StartOfRound.Instance.currentLevel.levelIncludesSnowFootprints && (artificeBlizzard == null || artificeBlizzard.activeSelf);
        }

        // repurposed from Buttery Fixes
        internal static void CacheCaveTiles()
        {
            caveTiles.Clear();

            if (RoundManager.Instance.currentDungeonType == 4)
            {
                GameObject dungeonRoot = RoundManager.Instance.dungeonGenerator?.Root ?? GameObject.Find("/Systems/LevelGeneration/LevelGenerationRoot");
                if (dungeonRoot == null)
                {
                    if (StartOfRound.Instance.currentLevel.name != "CompanyBuildingLevel")
                        Plugin.Logger.LogWarning("Landed on a moon with no dungeon generated. This shouldn't happen");

                    return;
                }

                foreach (Tile tile in dungeonRoot.GetComponentsInChildren<Tile>())
                {
                    if (tile.name.StartsWith("Cave"))
                    {
                        caveTiles.Add(tile.OverrideAutomaticTileBounds ? tile.transform.TransformBounds(tile.TileBoundsOverride) : tile.Bounds);
                        //Plugin.Logger.LogDebug($"Cached bounds of tile {tile.name}");
                    }
                }
            }
        }
    }
}
