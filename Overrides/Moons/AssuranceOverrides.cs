using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class AssuranceOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "MaskedPlayerEnemy",    3 },
            { "ClaySurgeon",          1 },
            { "Crawler",             75 },
            { "Centipede",          300 },
            { "Stingray",            40 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.assuranceNerfScrap.Value)
            {
                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v9
                    { "Cog1", 94 },
                    { "EnginePart1", 80 },
                    //{ "MetalSheet", 100 },
                    { "BigBolt", 89 },
                    { "ToyCube", 11 },
                    //{ "FlashLaserPointer", 4 },
                    { "BottleBin", 67 },
                    //{ "Remote", 10 },
                    //{ "RobotToy", 2 },
                    //{ "MagnifyingGlass", 4 },
                    //{ "Mug", 0 },
                    //{ "SodaCanRed", 2 },
                    //{ "Phone", 0 },
                    //{ "Hairdryer", 0 },
                    //{ "ClownHorn", 4 },
                    //{ "Airhorn", 4 },

                    // OFFENSE
                    { "MetalSheet", 65 },
                    //{ "ToyCube", 18 },
                    { "StopSign", 27 },
                    //{ "Bell", 0 },
                    //{ "CashRegister", 0 },
                    { "YieldSign", 28 },
                    { "DiyFlashbang", 13 },
                    //{ "TragedyMask", 0 },
                    { "ToiletPaperRolls", 18 },

                    // DINE
                    { "Candy", 50 }, // v72
                });
            }

            if (Configuration.assuranceMasked.Value)
                MoonOverrides.adjustedEnemies.Add("MaskedPlayerEnemy", 2);

            if (Configuration.assuranceGiants.Value)
                MoonOverrides.adjustedEnemies.Add("ForestGiant", 9);

            MoonOverrides.Apply(level);
        }
    }
}
