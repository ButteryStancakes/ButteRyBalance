namespace ButteRyBalance.Overrides.Enemies
{
    internal class ButlerOverrides
    {
        static int rollbackManor = -1;

        internal static void Overrides_BeforeSpawnWave()
        {
            if (Configuration.butlerManorChance.Value || rollbackManor >= 0)
            {
                if (Common.enemies.TryGetValue("Butler", out EnemyType butler))
                {
                    // increased manor chance setting
                    if (Configuration.butlerManorChance.Value)
                    {
                        if (butler.increasedChanceInterior != 1)
                        {
                            rollbackManor = butler.increasedChanceInterior;
                            butler.increasedChanceInterior = 1;
                            Plugin.Logger.LogDebug("Butler: Increased chance in manors");
                        }
                    }
                    else if (rollbackManor >= 0)
                    {
                        butler.increasedChanceInterior = rollbackManor;
                        rollbackManor = -1;
                        Plugin.Logger.LogDebug($"Butler: Increased chance in interior ID {butler.increasedChanceInterior} (-1 is none)");
                    }
                }
                else
                    Plugin.Logger.LogWarning("Failed to reference Butler enemy type. This should never happen");
            }
        }
    }
}
