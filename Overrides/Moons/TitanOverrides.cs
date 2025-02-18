using MonoMod.Utils;
using System.Collections.Generic;

namespace ButteRyBalance.Overrides.Moons
{
    internal class TitanOverrides
    {
        internal static readonly Dictionary<int, int> adjustedInteriors = new()
        {
            { 4, 69 },  // mineshaft,    vanilla: 115
        };

        internal static readonly Dictionary<string, int> infestations = new()
        {
            { "HoarderBug",         135 }, // 40
            { "Nutcracker",          45 }, // 140
            { "MaskedPlayerEnemy",  300 },
        };

        internal static void Setup(SelectableLevel level)
        {
            if (Configuration.titanBuffScrap.Value)
            {
                MoonOverrides.minScrap = 28; // vanilla: 28
                MoonOverrides.maxScrap = 36; // vanilla: 32

                MoonOverrides.adjustedScrap.AddRange(new(){
                    // DINE
                    { "Cog1", 19 },
                    { "EnginePart1", 19 },
                    { "FancyLamp", 29 },
                    { "FancyPainting", 44 },
                    { "Brush", 18 },
                    { "PerfumeBottle", 16 },
                    { "Mug", 44 },
                    { "7Ball", 30 }, // v49
                    { "DiyFlashbang", 10 },
                    { "WhoopieCushion", 18 },

                    // v45
                    { "GiftBox", 67 },
                });
            }

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

                    // REND
                    { "Jester", 60 },
                });
            }

            if (Configuration.titanWeeds.Value)
            {
                Plugin.Logger.LogDebug($"{level.name}.canSpawnMold: {level.canSpawnMold} -> False");
                level.canSpawnMold = false;
            }

            MoonOverrides.Apply(level);
        }
    }
}
