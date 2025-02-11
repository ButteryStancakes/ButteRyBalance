using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class AssuranceOverrides
    {
        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         300 },
            { "Nutcracker",         100 },
            { "MaskedPlayerEnemy",    1 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.assuranceNerfScrap.Value)
            {
                MoonOverrides.minScrap = 11; // vanilla: 13
                MoonOverrides.maxScrap = 16; // vanilla: 16

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // OFFENSE
                    { "Cog1", 94 },
                    { "EnginePart1", 80 },
                    //{ "MetalSheet", 65 },
                    { "BigBolt", 89 },
                    { "ToyCube", 18 },
                    { "StopSign", 27 },
                    { "Bell", 0 },
                    { "CashRegister", 0 },
                    { "YieldSign", 28 },
                    { "DiyFlashbang", 13 },
                    { "TragedyMask", 0 },
                    { "ToiletPaperRolls", 18 },
                            
                    // EMBRION
                    { "MetalSheet", 100 },

                    // VOW
                    { "WhoopieCushion", 49 }, // v49

                    // REND
                    { "Candy", 15 },
                });
            }

            if (Configuration.assuranceMasked.Value)
                MoonOverrides.adjustedEnemies.Add("MaskedPlayerEnemy", 2);

            MoonOverrides.Apply(level);
        }
    }
}
