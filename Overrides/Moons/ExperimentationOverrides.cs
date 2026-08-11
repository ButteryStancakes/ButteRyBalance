using ButteRyBalance.Network;
using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class ExperimentationOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "Crawler",             14 },
            { "Centipede",          300 },
            { "Stingray",            50 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.experimentationBuffScrap.Value)
            {
                MoonOverrides.minScrap = 11; // vanilla: 8
                MoonOverrides.maxScrap = 16; // vanilla: 12

                MoonOverrides.adjustedScrap.AddRange(new(){
                    { "CashRegister", 6 }, // v9
                    { "EasterEgg", 0 }, // early v50 betas
                });
            }

            if (BRBNetworker.Instance.ExperimentationNoEvents.Value)
                MoonOverrides.adjustedEnemies.Add("ForestGiant", 0);

            MoonOverrides.Apply(level);
        }
    }
}
