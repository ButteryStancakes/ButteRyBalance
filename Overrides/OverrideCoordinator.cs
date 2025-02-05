using ButteRyBalance.Overrides.Enemies;
using ButteRyBalance.Overrides.Moons;

namespace ButteRyBalance.Overrides
{
    internal class OverrideCoordinator
    {
        static bool initialized;

        static System.Action enemyRefreshOverrides, spawnWaveOverrides, leverPulledOverrides;

        internal static void Init()
        {
            if (initialized)
                return;

            initialized = true;

            enemyRefreshOverrides += BarberOverrides.Overrides_AfterEnemyRefresh;

            spawnWaveOverrides += ButlerOverrides.Overrides_BeforeSpawnWave;
            spawnWaveOverrides += DineOverrides.Overrides_BeforeSpawnWave;
        }

        internal static void AfterEnemyRefresh()
        {
            enemyRefreshOverrides?.Invoke();
        }

        internal static void BeforeSpawnWave()
        {
            spawnWaveOverrides?.Invoke();
        }

        internal static void BeforeGeneratingLevel()
        {
            leverPulledOverrides?.Invoke();
        }
    }
}
