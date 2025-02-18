using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class EmbrionOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 4, 118 }, // mineshaft,   vanilla: 44
        };
        internal static readonly Dictionary<int, int> megaInteriors = new()
        {
            { 0,  17 }, // factory,   vanilla: 300
            { 1,   4 }, // manor,     vanilla: 10
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "ClaySurgeon",        250 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.embrionBuffScrap.Value)
            {
                MoonOverrides.minScrap = 14; // vanilla: 14
                MoonOverrides.maxScrap = 18; // vanilla: 17

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v50
                    { "YieldSign", 12 },

                    // TITAN
                    { "Cog1", 37 },
                    { "EnginePart1", 40 },

                    // LIQUIDATION
                    { "BigBolt", 55 },
                    { "FlashLaserPointer", 10 },
                    { "Ring", 15 },
                    { "BottleBin", 42 },
                    { "RobotToy", 49 },
                    //{ "MagnifyingGlass", 37 },
                    { "TeaKettle", 23 },
                    { "Dentures", 37 },
                    { "Phone", 45 },
                    { "Airhorn", 39 },
                    { "ClownHorn", 31 },
                    //{ "DiyFlashbang", 13 },

                    { "MetalSheet", 40 },
                    { "DustPan", 83 },
                });
            }

            if (Configuration.embrionMega.Value)
            {
                MoonOverrides.minScrap = 28; // vanilla: 14
                MoonOverrides.maxScrap = 45; // vanilla: 17
            }

            if (Configuration.embrionAdjustEnemies.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    // non-biological
                    { "Nutcracker", 37 },
                    { "SpringMan", 42 },
                    { "ClaySurgeon", 61 },
                });
            }

            if (Configuration.embrionWeeds.Value)
            {
                Plugin.Logger.LogDebug($"{level.name}.canSpawnMold: {level.canSpawnMold} -> False");
                level.canSpawnMold = false;
            }

            MoonOverrides.Apply(level);
        }
    }
}
