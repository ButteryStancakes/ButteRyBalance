using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class TitanOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         135 },
            { "Nutcracker",          45 },
            { "MaskedPlayerEnemy",  300 },
            { "Crawler",            140 },
            { "Centipede",           28 },
            { "SpringMan",          100 },
            { "Stingray",           150 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.titanAddGold.Value)
                MoonOverrides.adjustedScrap.Add("GoldBar", 9);

            if (Configuration.titanAdjustEnemies.Value)
            {
                MoonOverrides.adjustedEnemies.AddRange(new(){
                    // v37
                    { "ForestGiant", 20 },
                    { "DressGirl", 28 },
                    // v50 betas
                    { "RadMech", 13 },
                    // v9
                    { "Blob", 41 },
                    { "HoarderBug", 55 },
                });
            }

            MoonOverrides.Apply(level);
        }
    }
}
