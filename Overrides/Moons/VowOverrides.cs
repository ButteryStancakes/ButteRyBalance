using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class VowOverrides
    {
        internal static readonly Dictionary<Common.InteriorID, int> adjustedInteriors = new()
        {
            { Common.InteriorID.Factory,     0 }, // vanilla: 300
            { Common.InteriorID.Mineshaft, 300 }, // vanilla: 192
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         151 },
            { "ClaySurgeon",        100 },
            { "Crawler",             44 },
            { "Centipede",          117 },
            { "SpringMan",           13 },
            { "Stingray",           250 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.vowMineshafts.Value)
            {
                if (level.maxOutsideEnemyPowerCount == 6)
                    MoonOverrides.outsidePowerCount = 7;

                MoonOverrides.minScrap = 10; // vanilla: 12
                MoonOverrides.maxScrap = 13; // vanilla: 15
            }

            if (Configuration.vowNoCoils.Value)
                MoonOverrides.adjustedEnemies.Add("SpringMan", 0);

            MoonOverrides.adjustedScrap.Add("Zeddog", 5); // v70

            MoonOverrides.Apply(level);
        }
    }
}
