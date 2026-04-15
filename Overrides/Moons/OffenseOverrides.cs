using MonoMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Overrides.Moons
{
    internal class OffenseOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         150 },
            { "Nutcracker",          50 },
            { "MaskedPlayerEnemy",  200 },
            { "Crawler",            300 },
            { "Centipede",          100 },
            { "SpringMan",          300 },
            { "Stingray",            44 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.offenseBuffScrap.Value)
            {
                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v49
                    { "YieldSign", 12 },

                    // ASSURANCE
                    { "Cog1", 30 },
                    { "EnginePart1", 40 },
                    { "MetalSheet", 23 },
                    { "BigBolt", 59 },
                    { "ToyCube", 31 },
                    { "StopSign", 40 }, // v38
                    { "GoldBar", 4 }, // v9
                    //{ "CashRegister", 3 },
                    { "DiyFlashbang", 14 },
                    //{ "TragedyMask", 3 },
                    //{ "SodaCanRed", 12 },
                    //{ "Bell", 16 },
                    { "ToiletPaperRolls", 19 },
                });
            }

            if (Configuration.offenseMasked.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    { "MaskedPlayerEnemy", 5 },
                    { "Flowerman", 0 }, // vanilla: 3
                    { "SpringMan", 25 }, // vanilla: 27
                });
            }

            if (Configuration.offenseNerfEclipse.Value)
                MoonOverrides.adjustedEclipse = 3; // vanilla: 4

            if (Configuration.offenseBees.Value)
            {
                MoonOverrides.adjustedEnemies.Add("RedLocustBees", 31);
                level.daytimeEnemySpawnChanceThroughDay = AnimationCurve.EaseInOut(0f, -1.272499f, 1f, -14.8181f);
            }

            if (Configuration.offenseNerfTraps.Value)
            {
                foreach (IndoorMapHazard indoorMapHazard in level.indoorMapHazards)
                {
                    foreach (SpawnableMapObject spawnableMapObject in level.spawnableMapObjects)
                    {
                        if (indoorMapHazard?.hazardType?.prefabToSpawn?.name == spawnableMapObject?.prefabToSpawn?.name)
                        {
                            indoorMapHazard.numberToSpawn = spawnableMapObject.numberToSpawn;
                            Plugin.Logger.LogDebug($"{level.name}.indoorMapHazards.{indoorMapHazard.hazardType.name}");
                        }
                    }
                }
            }

            if (Configuration.assuranceGiants.Value)
                MoonOverrides.adjustedEnemies.Add("ForestGiant", 1);

            MoonOverrides.Apply(level);
        }
    }
}
