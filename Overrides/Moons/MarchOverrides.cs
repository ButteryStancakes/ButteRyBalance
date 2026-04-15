using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class MarchOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         192 },
            { "Nutcracker",          64 },
            { "Crawler",            151 },
            { "Centipede",          150 },
            { "SpringMan",           28 },
            { "Stingray",           300 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.marchBuffScrap.Value)
            {
                MoonOverrides.minScrap = 14; // vanilla: 13
                MoonOverrides.maxScrap = 18; // vanilla: 17

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v9
                    { "ToyCube", 24 },
                    { "MagnifyingGlass", 21 },

                    // VOW
                    //{ "Cog1", 30 }, // v56
                    //{ "EnginePart1", 31 }, // v56
                    { "FishTestProp", 32 }, // v49
                });

                MoonOverrides.adjustedEnemies.Add("RedLocustBees", 72);
            }

            MoonOverrides.Apply(level);
        }
    }
}
