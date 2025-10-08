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
            if (Configuration.dineScrapPool.Value != Configuration.DineScrap.DontChange)
            {
                if (Configuration.dineScrapPool.Value == Configuration.DineScrap.Rollback)
                {
                    MoonOverrides.minScrap = 22; // v72: 22
                    MoonOverrides.maxScrap = 28; // v72: 26

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

                        // remove v73 scrap
                        { "SeveredHand", 0 },
                        { "SeveredBone", 0 },
                        { "SeveredBoneRib", 0 },
                        { "SeveredEar", 0 },
                        { "SeveredFoot", 0 },
                        { "SeveredThigh", 0 },
                        { "SeveredHeart", 0 },
                        { "SeveredTongue", 0 },
                    });
                }
                else
                {
                    // add v73 scrap
                    MoonOverrides.adjustedScrap.AddRange(new(){
                        { "SeveredHand", 100 },
                        { "SeveredBone", 79 },
                        { "SeveredBoneRib", 79 },
                        { "SeveredEar", 41 },
                        { "SeveredFoot", 100 },
                        { "SeveredThigh", 55 },
                        { "SeveredHeart", 6 },
                        { "SeveredTongue", 32 },
                    });

                    if (Configuration.dineScrapPool.Value == Configuration.DineScrap.AddV73 || Configuration.dineScrapPool.Value == Configuration.DineScrap.AddV73Extra)
                    {
                        if (Configuration.dineScrapPool.Value == Configuration.DineScrap.AddV73Extra)
                        {
                            MoonOverrides.minScrap = 28;
                            MoonOverrides.maxScrap = 45;
                        }
                        else
                        {
                            MoonOverrides.minScrap = 22;
                            MoonOverrides.maxScrap = 26;
                        }
                        MoonOverrides.adjustedScrap.AddRange(new(){
                            { "Cog1", 19 },
                            { "EnginePart1", 19 },
                            { "FancyLamp", 29 },
                            { "Bell", 21 },
                            { "Ring", 16 },
                            { "RobotToy", 17 },
                            { "Toothpaste", 41 },
                            { "Brush", 18 },
                            { "PillBottle", 29 },
                            { "PerfumeBottle", 16 },
                            { "BottleBin", 45 },
                            { "MagnifyingGlass", 14 },
                            { "Hairdryer", 14 },
                            { "TeaKettle", 43 },
                            { "Airhorn", 15 },
                            { "ClownHorn", 12 },
                            { "CashRegister", 14 },
                            { "Candy", 50 },
                            { "GiftBox", 11 },
                            { "TragedyMask", 30 },
                        });
                    }
                    else
                    {
                        MoonOverrides.minScrap = 200;
                        MoonOverrides.maxScrap = 250;
                        MoonOverrides.adjustedScrap.AddRange(new(){
                            { "Cog1", 0 },
                            { "EnginePart1", 0 },
                            { "FishTestProp", 0 },
                            { "BigBolt", 0 },
                            { "FancyLamp", 0 },
                            { "ToyCube", 0 },
                            { "PickleJar", 0 },
                            { "FlashLaserPointer", 0 },
                            { "FancyCup", 0 },
                            { "FancyPainting", 0 },
                            { "Bell", 0 },
                            { "Ring", 0 },
                            { "RobotToy", 0 },
                            { "Toothpaste", 0 },
                            { "Brush", 0 },
                            { "PillBottle", 0 },
                            { "PerfumeBottle", 0 },
                            { "Mug", 0 },
                            { "BottleBin", 0 },
                            { "MagnifyingGlass", 0 },
                            { "Hairdryer", 0 },
                            { "Phone", 0 },
                            { "SodaCanRed", 0 },
                            { "Dentures", 0 },
                            { "7Ball", 0 },
                            { "RubberDuck", 0 },
                            { "TeaKettle", 0 },
                            { "Airhorn", 0 },
                            { "ClownHorn", 0 },
                            { "CashRegister", 0 },
                            { "Candy", 0 },
                            { "DiyFlashbang", 0 },
                            { "GiftBox", 0 },
                            { "TragedyMask", 0 },
                            { "ComedyMask", 0 },
                            { "WhoopieCushion", 1 },
                            { "EasterEgg", 1 },
                            { "GarbageLid", 0 },
                            { "ToiletPaperRolls", 0 },
                            { "Zeddog", 0 },
                        });
                    }
                }

                if (Configuration.dineScrapPool.Value != Configuration.DineScrap.Consolidate)
                {
                    // unchanged in BRB v0.2.4
                    MoonOverrides.adjustedScrap.AddRange(new(){
                        { "FishTestProp", 5 },
                        { "BigBolt", 4 },
                        { "ToyCube", 33 },
                        { "PickleJar", 30 },
                        { "FlashLaserPointer", 5 },
                        { "FancyCup", 36 },
                        { "FancyPainting", 44 },
                        { "Mug", 48 },
                        { "Phone", 8 },
                        { "SodaCanRed", 50 },
                        { "Dentures", 44 },
                        { "7Ball", 24 },
                        { "RubberDuck", 25 },
                        { "DiyFlashbang", 20 },
                        { "ComedyMask", 47 },
                        { "WhoopieCushion", 12 },
                        { "EasterEgg", 44 },
                        { "GarbageLid", 22 },
                        { "ToiletPaperRolls", 28 },
                        { "Zeddog", 1 },
                    });
                }
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
