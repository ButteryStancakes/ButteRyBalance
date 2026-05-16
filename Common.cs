using ButteRyBalance.Network;
using DunGen;
using System.Collections.Generic;
using Unity.Netcode;
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

        internal static bool INSTALLED_ARTIFICE_BLIZZARD, INSTALLED_BARBER_FIXES, INSTALLED_SPAWN_CYCLE_FIXES, INSTALLED_VERSION55_COMPANY_CRUISER, INSTALLED_FAIRER_FIRE_EXITS;
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

        internal static VehicleController vehicleController;

        internal static List<EntranceTeleport> extraFireExits = [];
        internal static int fireExitCount = 0;

        internal static List<NetworkObject> tempNetObjs = [];

        internal static string lastSceneLoaded = string.Empty;

        internal static void Disconnect()
        {
            enemies.Clear();
            caveTiles.Clear();
            extraFireExits.Clear();
            CleanTemporaryNetworkObjects();
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

        internal static void CleanTemporaryNetworkObjects()
        {
            if (BRBNetworker.Instance.IsServer)
            {
                foreach (NetworkObject netObj in tempNetObjs)
                {
                    if (netObj == null)
                        continue;

                    Plugin.Logger.LogDebug($"Despawning \"{netObj.name}\" on server (temporary network object)");
                    netObj.Despawn(true);
                }
            }
            tempNetObjs.Clear();
        }
    }
}
