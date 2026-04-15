using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class EmbrionOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 0,  13 }, // factory,   vanilla: 300
            { 1,   4 }, // manor,     vanilla: 10
            { 4, 118 }, // mineshaft, vanilla: 44
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "ClaySurgeon",        250 },
            { "Crawler",            115 },
            { "Centipede",          140 },
            { "SpringMan",          150 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.embrionMega.Value)
            {
                MoonOverrides.minScrap = 28; // vanilla: 14
                MoonOverrides.maxScrap = 45; // vanilla: 17

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v50
                    { "YieldSign", 12 },
                    { "EasterEgg", 92 },

                    // LIQUIDATION
                    //{ "BigBolt", 55 },
                    { "FlashLaserPointer", 10 },
                    //{ "Ring", 15 },
                    //{ "BottleBin", 42 },
                    { "RobotToy", 49 },
                    { "MagnifyingGlass", 37 },
                    //{ "Dentures", 37 },
                    { "Phone", 45 },
                    { "Airhorn", 39 },
                    //{ "ClownHorn", 31 },
                    { "DiyFlashbang", 13 },
                });
            }

            MoonOverrides.Apply(level);
        }
    }
}
