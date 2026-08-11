using ButteRyBalance.Network;
using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class EmbrionOverrides
    {
        internal static readonly Dictionary<Common.InteriorID, int> adjustedInteriors = new()
        {
            { Common.InteriorID.Factory,    13 }, // vanilla: 300
            { Common.InteriorID.Manor,       4 }, // vanilla: 10
            { Common.InteriorID.Mineshaft, 118 }, // vanilla: 44
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
            if (BRBNetworker.Instance.EmbrionMega.Value)
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

                MoonOverrides.adjustedEnemies.AddRange(new(){
                    // non-biological
                    { "Nutcracker", 37 },
                    { "SpringMan", 42 },
                    { "ClaySurgeon", 61 },

                    { "HoarderBug", 57 },
                });
            }

            if (Configuration.embrionWeeds.Value)
                level.canSpawnMold = false;

            MoonOverrides.Apply(level);
        }
    }
}
