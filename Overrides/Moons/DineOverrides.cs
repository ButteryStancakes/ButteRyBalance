using ButteRyBalance.Network;
using System.Linq;

namespace ButteRyBalance.Overrides.Moons
{
    internal class DineOverrides
    {
        static int rollbackButler = -1;

        internal static void Overrides_BeforeSpawnWave()
        {
            if (RoundManager.Instance.currentLevel.name != "DineLevel")
                return;

            if (Configuration.dineReduceButlers.Value || rollbackButler >= 0)
            {
                if (Common.enemies.TryGetValue("Butler", out EnemyType butler))
                {
                    int butlerWeight = -1;

                    if (Configuration.dineReduceButlers.Value && !BRBNetworker.Instance.MoonsKillSwitch.Value)
                        butlerWeight = 15; // 15 * 1.7 = 25 (rounded down)
                    else if (rollbackButler >= 0)
                        butlerWeight = rollbackButler; // as of v69

                    if (butlerWeight >= 0)
                    {
                        SpawnableEnemyWithRarity butlerSpawns = RoundManager.Instance.currentLevel.Enemies.FirstOrDefault(enemy => enemy.enemyType == butler);
                        if (butlerSpawns != null)
                        {
                            if (butlerWeight == rollbackButler)
                                rollbackButler = -1;
                            else
                                rollbackButler = butlerSpawns.rarity;

                            if (butlerSpawns.rarity != butlerWeight)
                                Plugin.Logger.LogDebug($"Dine: Butler weight -> {butlerWeight}");

                            butlerSpawns.rarity = butlerWeight;
                        }
                    }
                }
                else
                    Plugin.Logger.LogWarning("Failed to reference Butler enemy type. This should never happen");
            }
        }
    }
}
