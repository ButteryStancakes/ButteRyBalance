using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class RendOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 4, 14 },  // mineshaft,    vanilla: 50
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "Nutcracker",         300 },
            { "MaskedPlayerEnemy",  150 },
            { "Butler",               4 },
            { "ClaySurgeon",         75 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.rendAdjustScrap.Value)
            {
                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v69 jolly
                    { "GiftBox", 49 },

                    // VOW
                    { "BottleBin", 58 },

                    // DINE
                    //{ "MagnifyingGlass", 14 },
                    { "PillBottle", 29 },
                    //{ "PerfumeBottle", 34 }, // v56
                    { "Toothpaste", 41 },
                    //{ "TeaKettle", 43 },
                    { "Candy", 50 },
                    //{ "ToiletPaperRolls", 28 },
                    { "CashRegister", 22 },

                    // TITAN
                    { "Brush", 31 },
                    { "7Ball", 16 },
                    { "WhoopieCushion", 26 },

                    // EMBRION
                    { "ControlPad", 14 },

                    // LIQUIDATION
                    { "TeaKettle", 47 },
                });
            }

            if (Configuration.rendAdjustIndoor.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    // TITAN
                    { "Jester", 71 },
                    // ARTIFICE
                    { "DressGirl", 28 }, // v50 beta
                    { "ClaySurgeon", 20 }, // v55

                    // v9
                    { "Blob", 24 },
                    { "Flowerman", 66 },

                    // counteract reduced coilhead curve
                    { "SpringMan", 70 },
                    // these are iconic and scary
                    { "MaskedPlayerEnemy", 43 }
                });
            }

            if (Configuration.rendWorms.Value)
                MoonOverrides.adjustedEnemies.Add("SandWorm", 18);

            MoonOverrides.Apply(level);
        }
    }
}
