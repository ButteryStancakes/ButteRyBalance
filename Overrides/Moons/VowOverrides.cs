using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class VowOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 0, 50 },  // factory,     vanilla: 300
            { 4, 300 }, // mineshaft,   vanilla: 192
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
            }

            if (Configuration.vowNoCoils.Value)
                MoonOverrides.adjustedEnemies.Add("SpringMan", 0);

            if (Configuration.vowNoTraps.Value)
            {
                level.indoorMapHazards = [];
                Plugin.Logger.LogDebug($"{level.name}.indoorMapHazards");
            }


            MoonOverrides.Apply(level);
        }
    }
}
