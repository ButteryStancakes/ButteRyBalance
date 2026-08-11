using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class DineOverrides
    {
        internal static readonly Dictionary<string, (int, int)> consolidatedValues = new()
        {
            { "SeveredBone",    (  60,  90 ) }, // vanilla: 17,  37
            { "SeveredBoneRib", (  70, 160 ) }, // vanilla: 20,  65
            { "SeveredEar",     (  25,  85 ) }, // vanilla:  7,  35
            { "SeveredFoot",    (  50, 135 ) }, // vanilla: 15,  55
            { "SeveredHand",    (  35,  75 ) }, // vanilla: 10,  30
            { "SeveredHeart",   ( 210, 440 ) }, // vanilla: 60, 250
            { "SeveredThigh",   (  70, 110 ) }, // vanilla: 20,  45
            { "SeveredTongue",  (  30, 100 ) }, // vanilla:  8,  40
        };

        internal static readonly Dictionary<Common.InteriorID, int> adjustedInteriors = new()
        {
            { Common.InteriorID.Mineshaft, 140 },  // vanilla: 17
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",           3 },
            { "Nutcracker",           1 },
            { "MaskedPlayerEnemy",   40 },
            { "Butler",             300 },
            { "ClaySurgeon",         14 },
            { "Crawler",             17 },
            { "Centipede",            8 },
            { "SpringMan",            4 },
            { "Stingray",            94 },
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
                        // v72
                        //{ "Cog1", 19 },
                        //{ "EnginePart1", 19 },
                        { "FishTestProp", 5 },
                        { "BigBolt", 4 },
                        { "FancyLamp", 29 },
                        { "ToyCube", 33 },
                        { "PickleJar", 30 },
                        { "FlashLaserPointer", 5 },
                        { "FancyCup", 36 },
                        //{ "FancyPainting", 44 },
                        { "Bell", 21 },
                        { "Ring", 16 },
                        { "RobotToy", 17 },
                        { "Toothpaste", 41 },
                        { "Brush", 18 },
                        { "PillBottle", 29 },
                        { "PerfumeBottle", 16 },
                        { "Mug", 48 },
                        //{ "BottleBin", 45 },
                        { "MagnifyingGlass", 14 },
                        { "Hairdryer", 14 },
                        { "Phone", 8 },
                        { "SodaCanRed", 50 },
                        { "Dentures", 44 },
                        //{ "7Ball", 24 },
                        { "RubberDuck", 25 },
                        { "TeaKettle", 43 },
                        //{ "Airhorn", 15 },
                        //{ "ClownHorn", 12 },
                        //{ "CashRegister", 14 },
                        { "Candy", 50 },
                        { "DiyFlashbang", 20 },
                        //{ "GiftBox", 11 },
                        //{ "TragedyMask", 30 },
                        { "ComedyMask", 47 },
                        //{ "WhoopieCushion", 12 },
                        { "EasterEgg", 44 },
                        { "GarbageLid", 22 },
                        { "ToiletPaperRolls", 28 },
                        //{ "Zeddog", 1 },

                        // v69 jolly
                        //{ "GiftBox", 41 },

                        // v56
                        { "Cog1", 15 },
                        { "EnginePart1", 14 },
                        //{ "FancyLamp", 54 },
                        //{ "PickleJar", 13 },
                        { "FancyPainting", 35 },
                        //{ "Bell", 48 },
                        //{ "Ring", 26 },
                        //{ "RobotToy", 26 },
                        //{ "Brush", 22 },
                        //{ "PillBottle", 14 },
                        //{ "PerfumeBottle", 34 },
                        //{ "Mug", 44 },
                        { "BottleBin", 30 },
                        //{ "Hairdryer", 22 },
                        //{ "Phone", 12 },
                        { "7Ball", 30 },
                        //{ "RubberDuck", 18 },
                        { "Airhorn", 16 },
                        { "ClownHorn", 17 },
                        { "CashRegister", 22 },
                        { "WhoopieCushion", 18 },

                        // v49
                        //{ "FancyPainting", 50 },
                        //{ "Airhorn", 11 },
                        //{ "ClownHorn", 11 },
                        //{ "Candy", 16 },
                        //{ "DiyFlashbang", 10 },
                        //{ "GiftBox", 21 },
                        { "TragedyMask", 64 },
                        //{ "ComedyMask", 29 },

                        // v45
                        { "GiftBox", 69 },

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
                    MoonOverrides.minScrap = 38;
                    MoonOverrides.maxScrap = 75;

                    MoonOverrides.adjustedScrap.AddRange(new(){
                        { "WhoopieCushion", 1 },
                        { "EasterEgg", 1 },

                        // add v73 scrap
                        { "SeveredHand", 100 },
                        { "SeveredBone", 79 },
                        { "SeveredBoneRib", 79 },
                        { "SeveredEar", 41 },
                        { "SeveredFoot", 100 },
                        { "SeveredThigh", 55 },
                        { "SeveredHeart", 6 },
                        { "SeveredTongue", 32 },
                        
                        // remove normal scrap
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
                        { "GarbageLid", 0 },
                        { "ToiletPaperRolls", 0 },
                        //{ "Zeddog", 0 },
                    });
                }

                MoonOverrides.adjustedScrap.Add("Zeddog", 1);
            }

            if (Configuration.dineAdjustEnemies.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    { "ForestGiant", 28 }, // vanilla: 100
                    { "RadMech", 13 }, // vanilla: 3
                });

                MoonOverrides.outsidePowerCount = 10; // vanilla: 9

                MoonOverrides.powerCount = 15; // vanilla: 10
            }

            MoonOverrides.Apply(level);
        }
    }
}
