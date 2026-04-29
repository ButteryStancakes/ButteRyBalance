using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class RendOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "Nutcracker",         300 },
            { "MaskedPlayerEnemy",  200 },
            { "Butler",               4 },
            { "ClaySurgeon",         75 },
            { "Centipede",          100 },
            { "SpringMan",          150 },
            { "Stingray",            11 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.rendWorms.Value)
                MoonOverrides.adjustedEnemies.Add("SandWorm", 18);

            MoonOverrides.adjustedScrap.Add("Zeddog", 8);

            MoonOverrides.Apply(level);
        }
    }
}
