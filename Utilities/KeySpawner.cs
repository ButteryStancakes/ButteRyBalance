using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Utilities
{
    internal static class KeySpawner
    {
        internal static void PostProcessKeyNodes(ref GameObject[] nodes, ref int count)
        {
            if (!RoundManager.Instance.IsServer || !Configuration.cavernsNoKeys.Value)
                return;

            List<GameObject> tunnelNodes = new(nodes);

            for (int i = tunnelNodes.Count - 1; i >= 0; i--)
            {
                if (tunnelNodes[i] == null)
                {
                    tunnelNodes.RemoveAt(i);
                    continue;
                }

                foreach (Bounds caveTile in Common.caveTiles)
                {
                    if (caveTile.Contains(tunnelNodes[i].transform.position))
                    {
                        tunnelNodes.RemoveAt(i);
                        continue;
                    }
                }

                foreach (GameObject caveNode in RoundManager.Instance.allCaveNodes)
                {
                    if (Vector3.Distance(tunnelNodes[i].transform.position, caveNode.transform.position) < 12f)
                    {
                        tunnelNodes.RemoveAt(i);
                        continue;
                    }
                }
            }

            Plugin.Logger.LogDebug($"Key spawn nodes: {count} -> {tunnelNodes.Count}");

            nodes = tunnelNodes.ToArray();
            count = nodes.Length;
        }
    }
}
