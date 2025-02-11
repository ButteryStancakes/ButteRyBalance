using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class AdamanceOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "MaskedPlayerEnemy",    3 },
            { "Butler",               1 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.adamanceBuffScrap.Value)
            {
                MoonOverrides.minScrap = 19; // vanilla: 16
                MoonOverrides.maxScrap = 24; // vanilla: 19
            }

            if (Configuration.adamanceReduceChaos.Value)
            {
                MoonOverrides.outsidePowerCount = 10; // vanilla: 13
                MoonOverrides.adjustedEclipse = 2; // vanilla: 3
            }

            MoonOverrides.Apply(level);
        }
    }
}
