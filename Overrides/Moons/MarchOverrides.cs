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
                    // VOW
                    { "Cog1", 25 }, // v60
                    { "EnginePart1", 25 }, // v60
                    { "BottleBin", 30 },
                    { "Flask", 49 }, // v56
                    { "ToiletPaperRolls", 34 },
                    { "PlasticCup", 22 },
                    { "Bell", 40 }, // v49

                    // OFFENSE
                    { "MetalSheet", 65 },

                    // LIQUIDATION
                    { "BigBolt", 55 },
                });

                MoonOverrides.adjustedEnemies.Add("RedLocustBees", 72);
            }

            MoonOverrides.Apply(level);
        }
    }
}
