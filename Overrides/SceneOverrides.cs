using ButteRyBalance.Network;
using DunGen;
using DunGen.Graph;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

namespace ButteRyBalance.Overrides
{
    internal static class SceneOverrides
    {
        static GameObject fireExitPrefab;
        internal static EntranceTeleport entranceTeleport1, entranceTeleport2, entranceTeleport3;

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive)
                return;

            string fogPath = "/Environment/Lighting/BrightDay/Local Volumetric Fog";

            entranceTeleport1 = null;
            entranceTeleport2 = null;
            entranceTeleport3 = null;

            Common.lastSceneLoaded = scene.name;
            switch (Common.lastSceneLoaded)
            {
                case "Level3Vow":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value && BRBNetworker.Instance.VowMisty.Value)
                    {
                        LocalVolumetricFog localVolumetricFog = GameObject.Find(fogPath)?.GetComponent<LocalVolumetricFog>();
                        if (localVolumetricFog != null)
                            localVolumetricFog.parameters.meanFreePath = 15.1f;
                    }

                    break;

                case "Level4March":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
                    {
                        string teleportsPath = "/Environment/Teleports";
                        Transform teleports = GameObject.Find(teleportsPath)?.transform;

                        if (teleports != null)
                        {
                            entranceTeleport1 = teleports.Find("EntranceTeleportB")?.GetComponent<EntranceTeleport>();
                            entranceTeleport2 = teleports.Find("EntranceTeleportC")?.GetComponent<EntranceTeleport>();
                            entranceTeleport3 = teleports.Find("EntranceTeleportD")?.GetComponent<EntranceTeleport>();
                            if (entranceTeleport1 != null && entranceTeleport2 != null && entranceTeleport3 != null)
                            {
                                Plugin.Logger.LogDebug($"{scene.name}{teleportsPath}/{entranceTeleport1.name}.entranceId: {entranceTeleport1.entranceId} -> 3");
                                entranceTeleport1.entranceId = 3;
                                Plugin.Logger.LogDebug($"{scene.name}{teleportsPath}/{entranceTeleport2.name}.entranceId: {entranceTeleport2.entranceId} -> 1");
                                entranceTeleport2.entranceId = 1;
                                Plugin.Logger.LogDebug($"{scene.name}{teleportsPath}/{entranceTeleport3.name}.entranceId: {entranceTeleport3.entranceId} -> 2");
                                entranceTeleport3.entranceId = 2;
                                break;
                            }
                        }

                        Plugin.Logger.LogError("Failed to overwrite fire exit values on March");
                    }

                    break;

                case "Level5Rend":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
                    {
                        GameObject localVolumetricFog2 = GameObject.Find(fogPath + " (2)");
                        // this fog is placed randomly next to the ship and makes the fire exit more dangerous than it needs to be
                        if (localVolumetricFog2 != null)
                            localVolumetricFog2.SetActive(false);
                    }

                    break;

                case "Level6Dine":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value && BRBNetworker.Instance.DineFireExits.Value)
                    {
                        Transform cube002 = GameObject.Find("/Environment/Map/CementFacility1/Cube.002")?.transform;
                        if (cube002 != null)
                        {
                            Transform cube002Clone = Object.Instantiate(cube002, cube002.parent);
                            cube002Clone.SetLocalPositionAndRotation(new(-15.0400019f, 5.42199993f, 29.9410019f), Quaternion.Euler(-90.156f, 86.789f, 112.546f));
                            cube002Clone.localScale = new(0.683414161f, 0.899168372f, 0.683414578f);
                            GameObject cube = cube002Clone.Find("Cube")?.gameObject;
                            if (cube != null)
                                cube.SetActive(false);

                            Object.Instantiate(cube002Clone, cube002Clone.parent).SetLocalPositionAndRotation(new(21.4799995f, 1.69000006f, 54.7699966f), Quaternion.Euler(-89.907f, 0f, 173.213f));

                            Transform fireExitDoorContainer = GameObject.Find("/Environment/FireExitDoorContainer")?.transform;
                            if (fireExitDoorContainer != null)
                            {
                                Object.Instantiate(fireExitDoorContainer, fireExitDoorContainer.parent).SetLocalPositionAndRotation(new(-13.7140465f, 0.0367336273f, -67.9359283f), Quaternion.Euler(-0.072f, 136.001f, 0.003f));
                                Object.Instantiate(fireExitDoorContainer, fireExitDoorContainer.parent).SetLocalPositionAndRotation(new(149.996094f, -17.1400433f, 51.1220703f), Quaternion.Euler(0f, 109.805f, 0f));
                            }
                        }

                        entranceTeleport3 = SpawnFireExit(3, new(49.488266f, 9.30028534f, 62.257103f), Quaternion.Euler(-0.035f, 109.308f, -0.063f));
                        entranceTeleport2 = SpawnFireExit(2, new(-115.096237f, -7.86410809f, -50.7210159f), Quaternion.Euler(0f, 83.113f, 0f));
                        entranceTeleport1 = GameObject.Find("/Environment/Teleports/EntranceTeleportB")?.GetComponent<EntranceTeleport>();
                    }

                    break;

                case "Level7Offense":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value && BRBNetworker.Instance.OffenseFireExits.Value)
                    {
                        Transform cube002 = GameObject.Find("/Environment/Map/CementFacility1/Cube.002")?.transform;
                        if (cube002 != null)
                        {
                            Object.Instantiate(cube002, cube002.parent).SetLocalPositionAndRotation(new(4.86000061f, 1.82000029f, -30.0899887f), Quaternion.Euler(-89.901f, 0f, 175.644f));

                            Transform fireExitDoorContainer = GameObject.Find("/Environment/FireExitDoorContainer")?.transform;
                            if (fireExitDoorContainer != null)
                                Object.Instantiate(fireExitDoorContainer, fireExitDoorContainer.parent).SetLocalPositionAndRotation(new(28.0300026f, -2.22000027f, 47.7900009f), Quaternion.Euler(0f, 102.006f, 0f));
                        }

                        string teleportBPath = "/Environment/Teleports/EntranceTeleportB";
                        entranceTeleport2 = GameObject.Find(teleportBPath)?.GetComponent<EntranceTeleport>();
                        if (entranceTeleport2 != null)
                        {
                            Plugin.Logger.LogDebug($"{scene.name}{teleportBPath}.entranceId: {entranceTeleport2.entranceId} -> 2");
                            entranceTeleport2.entranceId = 2;

                            entranceTeleport1 = SpawnFireExit(1, new(113.459213f, 6.9392705f, -171.7966f), Quaternion.Euler(0f, -79.002f, 0f));
                        }
                    }

                    break;

                case "Level10Adamance":

                    if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
                    {
                        if (RoundManager.Instance.mapPropsContainer == null)
                            RoundManager.Instance.mapPropsContainer = GameObject.FindGameObjectWithTag("MapPropsContainer");

                        Object.Instantiate(RoundManager.Instance.quicksandPrefab, new(-117.796883f, -23.5f, 51.9545212f), Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                    }

                    break;
            }
        }

        static EntranceTeleport SpawnFireExit(int id, Vector3 pos, Quaternion rot)
        {
            if (!RoundManager.Instance.IsServer)
                return null;

            if (fireExitPrefab == null)
                CacheFireExitPrefab();

            if (fireExitPrefab == null)
            {
                Plugin.Logger.LogError("Failed to discover fire exit prefab, this will cause problems in gameplay");
                return null;
            }

            if (RoundManager.Instance.mapPropsContainer == null)
                RoundManager.Instance.mapPropsContainer = GameObject.FindGameObjectWithTag("MapPropsContainer");

            NetworkObject fireExit = Object.Instantiate(fireExitPrefab, pos, rot, RoundManager.Instance.mapPropsContainer.transform).GetComponent<NetworkObject>();
            fireExit.Spawn(true);
            Common.tempNetObjs.Add(fireExit);
            BRBNetworker.Instance.SyncFireExitRpc(fireExit, id);

            return fireExit.GetComponent<EntranceTeleport>();
        }

        static void CacheFireExitPrefab()
        {
            IndoorMapType factory = RoundManager.Instance.dungeonFlowTypes.FirstOrDefault(dungeonFlowType => dungeonFlowType?.dungeonFlow?.name == "Level1Flow");
            if (factory == null)
                return;

            foreach (GraphNode node in factory.dungeonFlow.Nodes)
            {
                foreach (TileSet tileSet in node.TileSets)
                {
                    if (tileSet.name != "Level1RoomTiles")
                        continue;

                    GameObject mediumRoomHallway1B = tileSet.TileWeights.Weights.FirstOrDefault(weight => weight.Value?.name == "MediumRoomHallway1B")?.Value;
                    if (mediumRoomHallway1B == null)
                        return;

                    Doorway doorway = mediumRoomHallway1B.GetComponentInChildren<Doorway>();
                    if (doorway == null)
                        return;

                    GameObject normalDoorBlockerWhiteCement = doorway.BlockerPrefabWeights.FirstOrDefault(blockerPrefabWeight => blockerPrefabWeight.GameObject?.name == "NormalDoorBlockerWhiteCement").GameObject;
                    if (normalDoorBlockerWhiteCement == null)
                        return;

                    fireExitPrefab = normalDoorBlockerWhiteCement.GetComponentInChildren<SpawnSyncedObject>()?.spawnPrefab;
                    return;
                }
            }
        }
    }
}
