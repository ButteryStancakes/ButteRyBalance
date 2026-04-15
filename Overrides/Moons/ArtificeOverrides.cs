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
            { "HoarderBug",         207 },
            { "Nutcracker",          69 },
            { "MaskedPlayerEnemy",  151 },
            { "Butler",             100 },
            { "ClaySurgeon",         64 },
            { "Crawler",             48 },
            { "Centipede",           50 },
            { "SpringMan",            1 },
            { "Stingray",            75 },
        };

        internal static void Setup(SelectableLevel level)
        {
            // v56
            if (Configuration.artificeBuffScrap.Value)
                MoonOverrides.adjustedScrap.Add("GoldBar", 36); // vanilla: 32

            if (Configuration.artificeTurrets.Value)
            {
                IndoorMapHazard artificeTurrets = level.indoorMapHazards.FirstOrDefault(indoorMapHazard => indoorMapHazard.hazardType?.prefabToSpawn?.name == "TurretContainer");
                if (artificeTurrets != null)
                {
                    // Artifice's old turret curve is duplicated in Liquidation's level file
                    AnimationCurve liquidationTurrets = StartOfRound.Instance.levels.FirstOrDefault(x => x.name == "LiquidationLevel")?.spawnableMapObjects.FirstOrDefault(spawnableMapObject => spawnableMapObject.prefabToSpawn?.name == "TurretContainer")?.numberToSpawn;
                    if (liquidationTurrets != null)
                    {
                        artificeTurrets.numberToSpawn = liquidationTurrets;
                        Plugin.Logger.LogDebug($"{level.name}.indoorMapHazards.{artificeTurrets.hazardType.name}");
                    }
                }
            }

            MoonOverrides.Apply(level);
        }
    }
}
