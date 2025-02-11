using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class VowOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 0, 75 },  // factory,     vanilla: 300
            { 4, 300 }, // mineshaft,   vanilla: 250
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.vowAdjustScrap.Value)
            {
                MoonOverrides.adjustedScrap.AddRange(new(){
                    // v56
                    { "Cog1", 30 },
                    { "EnginePart1", 31 },
                    // MARCH
                    { "Flask", 42 },
                    { "ToiletPaperRolls", 32 },
                    { "PlasticCup", 27 },
                    // ASSURANCE
                    { "Bell", 16 },
                    // REND
                    { "BottleBin", 46 },
                });
            }

            if (Configuration.vowNoCoils.Value)
                MoonOverrides.adjustedEnemies.Add("SpringMan", 0);

            if (Configuration.vowNoTraps.Value)
            {
                level.spawnableMapObjects = [];
                Plugin.Logger.LogDebug($"{level.name}.spawnableMapObjects");
            }

            if (level.maxOutsideEnemyPowerCount == 6)
                MoonOverrides.outsidePowerCount = 7;

            MoonOverrides.Apply(level);
        }
    }
}
