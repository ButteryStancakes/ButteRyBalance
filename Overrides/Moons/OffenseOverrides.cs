using MonoMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Overrides.Moons
{
    internal class OffenseOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 0, 150 }, // factory,     vanilla: 300
            { 4, 300 }, // mineshaft,   vanilla: 200
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         150 }, // 100
            { "Nutcracker",          50 }, // 150
            { "MaskedPlayerEnemy",  300 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.offenseBuffScrap.Value)
            {
                MoonOverrides.minScrap = 14; // vanilla: 14
                MoonOverrides.maxScrap = 19; // vanilla: 18

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v49
                    { "YieldSign", 12 },

                    // ASSURANCE
                    { "Cog1", 30 },
                    { "EnginePart1", 40 },
                    { "MetalSheet", 23 },
                    //{ "BigBolt", 59 },
                    { "ToyCube", 31 },
                    { "StopSign", 40 }, // v38
                    { "CashRegister", 3 },
                    { "DiyFlashbang", 14 },
                    { "TragedyMask", 3 },
                    { "SodaCanRed", 12 },
                    { "Bell", 16 },
                    { "GoldBar", 4 }, // v9

                    // VOW
                    { "Mug", 18 },
                    { "BottleBin", 30 },
                    { "ClownHorn", 35 },
                    { "RubberDuck", 24 },
                });
            }

            if (Configuration.offenseMasked.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    { "MaskedPlayerEnemy", 9 },
                    { "SpringMan", 16 }
                });
            }

            if (Configuration.offenseNerfEclipse.Value)
                MoonOverrides.adjustedEclipse = 3; // vanilla: 4

            if (Configuration.offenseBees.Value)
            {
                MoonOverrides.adjustedEnemies.Add("RedLocustBees", 31);
                level.daytimeEnemySpawnChanceThroughDay = AnimationCurve.EaseInOut(0f, -1.272499f, 1f, -14.8181f);
            }

            MoonOverrides.Apply(level);
        }
    }
}
