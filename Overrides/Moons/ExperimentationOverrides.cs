using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class ExperimentationOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
        };

        internal static void Setup(SelectableLevel level)
        {
            level.riskLevel = "D"; // vanilla: B

            if (Configuration.experimentationBuffScrap.Value)
            {
                MoonOverrides.minScrap = 11; // vanilla: 8
                MoonOverrides.maxScrap = 16; // vanilla: 12

                MoonOverrides.adjustedScrap.Add("CashRegister", 6); // v9
            }

            if (Configuration.experimentationNoEggs.Value)
                MoonOverrides.adjustedScrap.Add("EasterEgg", 0);

            if (Configuration.experimentationNoGiants.Value)
                MoonOverrides.adjustedEnemies.Add("ForestGiant", 0);

            if (Configuration.experimentationNoNuts.Value)
                MoonOverrides.adjustedEnemies.Add("Nutcracker", 0);

            MoonOverrides.Apply(level);
        }
    }
}
