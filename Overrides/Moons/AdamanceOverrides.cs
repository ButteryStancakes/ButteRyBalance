using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class AdamanceOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 1,  17 }, // manor,     vanilla: 8
            { 4, 135 }, // mineshaft, vanilla: 48
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         225 },
            { "Nutcracker",          75 },
            { "MaskedPlayerEnemy",    1 },
            { "Butler",              10 },
            { "ClaySurgeon",         50 },
            { "Crawler",            300 },
            { "Centipede",           64 },
            { "SpringMan",           40 },
            { "Stingray",           200 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.adamanceBuffScrap.Value)
                MoonOverrides.adjustedScrap.Add("EasterEgg", 71); // vanilla: 50

            if (Configuration.adamanceNerfEclipse.Value)
                MoonOverrides.adjustedEclipse = 2; // vanilla: 3

            /*if (Configuration.adamanceReduceCadavers.Value && level.specialEnemyRarity != null && level.specialEnemyRarity.overrideEnemy != null && level.specialEnemyRarity.overrideEnemy.name == "CadaverGrowths")
            {
                level.specialEnemyRarity.overrideEnemy = null;
                level.specialEnemyRarity.percentageChance = 0f;
            }*/

            if (Configuration.adamanceInteriors.Value)
                MoonOverrides.adjustedEnemies.Add("Butler", 10);

            if (Configuration.adamanceNoMasks.Value)
                MoonOverrides.adjustedEnemies.Add("MaskedPlayerEnemy", 0); // vanilla: 2

            MoonOverrides.Apply(level);
        }
    }
}
