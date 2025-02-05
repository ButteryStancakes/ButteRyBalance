using System.Collections.Generic;

namespace ButteRyBalance
{
    internal class Common
    {
        internal static Dictionary<string, EnemyType> enemies = [];

        internal static void Disconnect()
        {
            enemies.Clear();
        }
    }
}
