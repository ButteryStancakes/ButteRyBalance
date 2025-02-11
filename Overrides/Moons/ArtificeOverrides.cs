using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ButteRyBalance.Overrides.Moons
{
    internal class ArtificeOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 0,  94 }, // factory, vanilla: 64
            { 1, 300 }, // manor,   vanilla: 151
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         100 },
            { "Nutcracker",         100 },
            { "MaskedPlayerEnemy",  100 },
            { "Butler",             100 },
        };

        internal static void Setup(SelectableLevel level)
        {
            // v56
            if (Configuration.artificeBuffScrap.Value)
            {
                MoonOverrides.minScrap = 31; // vanilla: 26
                MoonOverrides.maxScrap = 38; // vanilla: 31

                MoonOverrides.adjustedScrap.Add("GoldBar", 36); // vanilla: 32
            }

            if (Configuration.artificeTurrets.Value)
            {
                SpawnableMapObject artificeTurrets = level.spawnableMapObjects.FirstOrDefault(spawnableMapObject => spawnableMapObject.prefabToSpawn?.name == "TurretContainer");
                if (artificeTurrets != null)
                {
                    // Artifice's old turret curve is duplicated in Liquidation's level file
                    AnimationCurve liquidationTurrets = StartOfRound.Instance.levels.FirstOrDefault(x => x.name == "LiquidationLevel")?.spawnableMapObjects.FirstOrDefault(spawnableMapObject => spawnableMapObject.prefabToSpawn?.name == "TurretContainer")?.numberToSpawn;
                    if (liquidationTurrets != null)
                    {
                        artificeTurrets.numberToSpawn = liquidationTurrets;
                        Plugin.Logger.LogDebug($"{level.name}.spawnableMapObjects.{artificeTurrets.prefabToSpawn.name}");
                    }
                }
            }

            MoonOverrides.Apply(level);
        }
    }
}
