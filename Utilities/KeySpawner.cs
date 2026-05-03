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

            try
            {
                List<GameObject> tunnelNodes = new(nodes);
                List<GameObject> backupNodes = [];

                if (tunnelNodes.Count < 1)
                {
                    Plugin.Logger.LogWarning("Key spawner was fed an invalid array of nodes - this shouldn't happen");
                    tunnelNodes = new(RoundManager.Instance.insideAINodes);
                }

                bool goNext = false;
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
                            goNext = true;
                            break;
                        }
                    }

                    if (goNext)
                    {
                        goNext = false;
                        continue;
                    }

                    foreach (GameObject caveNode in RoundManager.Instance.allCaveNodes)
                    {
                        float dist = Vector3.Distance(tunnelNodes[i].transform.position, caveNode.transform.position);
                        if (dist < 12f)
                        {
                            if (dist > 8f)
                                backupNodes.Add(tunnelNodes[i]);

                            tunnelNodes.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (tunnelNodes.Count < 1)
                {
                    if (backupNodes.Count > 0)
                    {
                        Plugin.Logger.LogDebug($"Key spawn nodes: {count} -> {backupNodes.Count} (BACKUP)");

                        nodes = backupNodes.ToArray();
                        count = nodes.Length;
                        return;
                    }

                    Plugin.Logger.LogWarning("Ignoring \"No Keys in Caverns\" for this round, because there are no valid nodes outside of cavern tiles");
                    return;
                }

                Plugin.Logger.LogDebug($"Key spawn nodes: {count} -> {tunnelNodes.Count}");

                nodes = tunnelNodes.ToArray();
                count = nodes.Length;
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError("An error occurred while filtering key spawns");
                Plugin.Logger.LogError(e);
            }
        }
    }
}
