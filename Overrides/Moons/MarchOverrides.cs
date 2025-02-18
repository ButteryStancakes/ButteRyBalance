using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class MarchOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
        };

        internal static void Setup(SelectableLevel level)
        {
            level.riskLevel = "C"; // vanilla: B

            if (Configuration.marchBuffScrap.Value)
            {
                MoonOverrides.minScrap = 16; // vanilla: 13
                MoonOverrides.maxScrap = 21; // vanilla: 17

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v9
                    { "BigBolt", 59 },
                    { "ToyCube", 24 },
                    { "FlashLaserPointer", 8 },
                    { "Remote", 16 },
                    { "RobotToy", 6 },
                    { "MagnifyingGlass", 21 },
                    { "Mug", 12 },
                    { "SodaCanRed", 12 },
                    { "Phone", 7 },
                    { "Hairdryer", 4 },

                    // VOW
                    { "Cog1", 25 },
                    { "EnginePart1", 25 },
                    { "FishTestProp", 32 }, // v49
                    { "EggBeater", 50 }, // v49
                    { "ToiletPaperRolls", 34 },
                    { "PlasticCup", 22 },
                    { "Bell", 28 },

                    // OFFENSE
                    { "MetalSheet", 65 },
                    { "BottleBin", 46 },
                });

                MoonOverrides.adjustedEnemies.Add("RedLocustBees", 72);
            }

            MoonOverrides.Apply(level);
        }
    }
}
