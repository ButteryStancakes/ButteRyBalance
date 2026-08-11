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
            MoonOverrides.Apply(level);
        }
    }
}
