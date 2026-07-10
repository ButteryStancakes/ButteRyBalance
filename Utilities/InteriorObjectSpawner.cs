using DunGen;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Utilities
{
    internal static class InteriorObjectSpawner
    {
        internal static void PostProcessKeyNodes(ref GameObject[] nodes, ref int count)
        {
            if (!RoundManager.Instance.IsServer || !Configuration.cavernsNoKeys.Value)
                return;

            try
            {
                List<GameObject> tunnelNodes = new(nodes);
                List<GameObject> caveNodes = new(RoundManager.Instance.allCaveNodes);

                if (tunnelNodes.Count < 1)
                {
                    Plugin.Logger.LogWarning("Key spawner was fed an invalid array of nodes - this shouldn't happen");
                    tunnelNodes = new(RoundManager.Instance.insideAINodes);
                }

                for (int i = tunnelNodes.Count - 1; i >= 0; i--)
                {
                    foreach (Bounds caveTile in Common.caveTiles)
                    {
                        if (caveTile.Contains(tunnelNodes[i].transform.position) /*|| Vector3.Distance(tunnelNodes[i].transform.position, caveTile.center) < 12f*/)
                        {
                            caveNodes.Add(tunnelNodes[i]);
                            tunnelNodes.RemoveAt(i);
                            break;
                        }
                    }
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

                    foreach (GameObject caveNode in caveNodes)
                    {
                        float dist = Vector3.Distance(tunnelNodes[i].transform.position, caveNode.transform.position);
                        if (dist < 12f)
                        {
                            tunnelNodes.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (tunnelNodes.Count < 1)
                {
                    Plugin.Logger.LogWarning("Ignoring \"No Keys in Caverns\" for this round, because there aren't valid nodes outside exclusion criteria");
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

        internal static void RandomlyActivateFireExits(GlobalProp[] fireExits, System.Random rand)
        {
            List<GlobalProp> inactiveFireExits = new(fireExits);
            for (int i = 0; i < Common.fireExitCount; i++)
            {
                int index = rand.Next(inactiveFireExits.Count);
                inactiveFireExits[index].gameObject.SetActive(true);
                inactiveFireExits.RemoveAt(index);
            }
        }
    }
}
