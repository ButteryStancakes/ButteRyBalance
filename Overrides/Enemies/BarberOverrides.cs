namespace ButteRyBalance.Overrides.Enemies
{
    internal class BarberOverrides
    {
        static int rollbackGroup = -1, rollbackCount = -1;

        internal static void Overrides_AfterEnemyRefresh()
        {
            if (!RoundManager.Instance.IsServer)
                return;

            if (Configuration.barberDynamicSpawns.Value || rollbackGroup >= 0 || rollbackCount >= 0)
            {
                if (Common.enemies.TryGetValue("ClaySurgeon", out EnemyType barber))
                {
                    if (Configuration.barberDynamicSpawns.Value)
                    {
                        if (rollbackCount < 0)
                            rollbackCount = barber.MaxCount;
                        if (rollbackGroup < 0)
                            rollbackGroup = barber.spawnInGroupsOf;

                        if (RoundManager.Instance.currentDungeonType == 4)
                        {
                            barber.MaxCount = 1;
                            barber.spawnInGroupsOf = 1;
                            Plugin.Logger.LogDebug("Barber - Dynamic Spawn Settings: 1/1 (Mineshaft)");
                        }
                        else
                        {
                            barber.MaxCount = 8;
                            barber.spawnInGroupsOf = 2;
                            Plugin.Logger.LogDebug($"Barber - Dynamic Spawn Settings: 2/8 (Interior ID: {RoundManager.Instance.currentDungeonType})");
                        }
                    }
                    else
                    {
                        if (rollbackCount >= 0)
                        {
                            barber.MaxCount = rollbackCount;
                            rollbackCount = -1;
                            Plugin.Logger.LogDebug("Rolled back Barber max count");
                        }
                        if (rollbackGroup >= 0)
                        {
                            barber.spawnInGroupsOf = rollbackGroup;
                            rollbackGroup = -1;
                            Plugin.Logger.LogDebug("Rolled back Barber group");
                        }
                    }
                }
                else
                    Plugin.Logger.LogWarning("Failed to reference Barber enemy type. This should never happen");
            }
        }
    }
}
