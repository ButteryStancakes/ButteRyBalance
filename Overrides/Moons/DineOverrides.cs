using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class DineOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",           3 },
            { "Nutcracker",           1 },
            { "MaskedPlayerEnemy",   40 },
            { "Butler",             300 },
            { "ClaySurgeon",         14 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.dineBuffScrap.Value)
            {
                MoonOverrides.minScrap = 22; // vanilla: 22
                MoonOverrides.maxScrap = 28; // vanilla: 28

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v56
                    { "Cog1", 15 },
                    { "EnginePart1", 14 },
                    { "BottleBin", 30 },
                    { "FancyLamp", 54 },
                    { "Ring", 26 },
                    { "RobotToy", 26 },
                    { "PerfumeBottle", 34 },
                    //{ "Bell", 48 },
                    { "Hairdryer", 22 },
                    { "Airhorn", 16 },
                    { "ClownHorn", 17 },

                    // v49
                    //{ "FancyPainting", 50 },
                    { "CashRegister", 12 },
                    //{ "Candy", 16 },
                    //{ "GiftBox", 21 },
                    { "TragedyMask", 64 },

                    // v45
                    { "GiftBox", 69 },

                    // REND
                    //{ "MagnifyingGlass", 35 },
                    { "PillBottle", 4 },
                    //{ "PerfumeBottle", 28 },
                    { "Toothpaste", 24 },
                    { "TeaKettle", 25 },
                    //{ "7Ball", 23 },
                    { "Candy", 15 },
                    //{ "WhoopieCushion", 0 },
                    //{ "ToiletPaperRolls", 13 },
                            
                    // TITAN
                    { "Brush", 25 },

                    // LIQUIDATION
                    { "MagnifyingGlass", 37 },
                    { "Bell", 49 },

                    // get it?
                    { "DustPan", 32 },
                });
            }

            if (Configuration.dineReduceButlers.Value)
                MoonOverrides.adjustedEnemies.Add("Butler", 15); // vanilla: 24

            if (Configuration.dineAdjustIndoor.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
					// v56
                    { "DressGirl", 3 },
                    { "SandSpider", 7 },
                    { "Blob", 3 },
                    { "HoarderBug", 8 },
                    { "Jester", 5 },

                    // v50 betas
                    { "Centipede", 6 },
                });

                MoonOverrides.powerCount = 15; // vanilla: 16
            }

            if (Configuration.dineMasked.Value)
                MoonOverrides.adjustedEnemies.Add("MaskedPlayerEnemy", 4);

            if (Configuration.dineAdjustOutdoor.Value)
            {
                MoonOverrides.adjustedEnemies.Add("ForestGiant", 28); // vanilla: 100

                MoonOverrides.outsidePowerCount = 10; // vanilla: 7
            }

            if (Configuration.dineAdjustCurves.Value)
            {
                level.outsideEnemySpawnChanceThroughDay = new(
                    new(-7.7369623E-07f, -2.8875f),
                    new(0.47669196f, 0.6959345f),
                    new(1.0052626f, 5.3594007f));
                Plugin.Logger.LogDebug($"{level.name}.outsideEnemySpawnChanceThroughDay");
            }

            MoonOverrides.Apply(level);
        }
    }
}
