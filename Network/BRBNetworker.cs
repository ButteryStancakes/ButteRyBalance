using ButteRyBalance.Overrides;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace ButteRyBalance.Network
{
    internal class BRBNetworker : NetworkBehaviour
    {
        // --- INIT ---

        internal static GameObject prefab;
        internal static BRBNetworker Instance { get; private set; }

        internal static void Init()
        {
            if (prefab != null)
            {
                Plugin.Logger.LogDebug("Skipped network handler registration, because it has already been initialized");
                return;
            }

            try
            {
                // create "prefab" to hold our network references
                prefab = new(nameof(BRBNetworker))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                // assign a unique hash so it can be network registered
                NetworkObject netObj = prefab.AddComponent<NetworkObject>();
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(typeof(BRBNetworker).Assembly.GetName().Name + prefab.name + Plugin.PLUGIN_VERSION.ToString()));
                netObj.GlobalObjectIdHash = System.BitConverter.ToUInt32(hash, 0);

                // and now it holds our network handler!
                prefab.AddComponent<BRBNetworker>();

                // register it, and then it can be spawned
                NetworkManager.Singleton.AddNetworkPrefab(prefab);

                Plugin.Logger.LogDebug("Successfully registered network handler. This is good news!");
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError($"Encountered some fatal error while registering network handler. The mod will not function like this!\n{e}");
            }
        }

        internal static void Create()
        {
            try
            {
                if (NetworkManager.Singleton.IsServer && prefab != null)
                    Instantiate(prefab).GetComponent<NetworkObject>().Spawn(true);
            }
            catch
            {
                Plugin.Logger.LogError($"Encountered some fatal error while spawning network handler. It is likely that registration failed earlier on start-up, please consult your logs.");
            }
        }

        void Awake()
        {
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (Instance != this)
            {
                if (Instance.TryGetComponent(out NetworkObject netObj) && !netObj.IsSpawned && Instance != prefab)
                    Destroy(Instance);

                Plugin.Logger.LogWarning($"There are 2 {nameof(BRBNetworker)}s instantiated, and the wrong one was assigned as Instance. This shouldn't happen, but is recoverable");

                Instance = this;
            }
            Plugin.Logger.LogDebug("Successfully spawned network handler.");

            OverrideCoordinator.ApplyOnAllClients();
        }

        // --- NETWORKING ---

        void Start()
        {
            if (this != Instance || !IsSpawned)
                return;

            if (IsServer)
                UpdateConfig();
        }

        // config
        internal NetworkVariable<bool> JesterWalkThrough { get; private set; } = new();
        internal NetworkVariable<bool> JesterLongCooldown { get; private set; } = new();
        internal NetworkVariable<bool> ButlerStealthStab { get; private set; } = new();
        internal NetworkVariable<bool> ButlerLongCooldown { get; private set; } = new();
        internal NetworkVariable<bool> KnifeShortCooldown { get; private set; } = new();
        internal NetworkVariable<bool> ManeaterLimitGrowth { get; private set; } = new();
        internal NetworkVariable<bool> ManeaterWideTurns { get; private set; } = new();
        internal NetworkVariable<bool> MoonsKillSwitch { get; private set; } = new();
        internal NetworkVariable<bool> ExperimentationNoEvents { get; private set; } = new();
        internal NetworkVariable<bool> VowMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> RendShrink { get; private set; } = new();
        internal NetworkVariable<bool> DineFloods { get; private set; } = new();
        internal NetworkVariable<bool> JetpackBattery { get; private set; } = new();
        internal NetworkVariable<bool> ArtificeInteriors { get; private set; } = new();
        internal NetworkVariable<bool> ZapGunPrice { get; private set; } = new();
        internal NetworkVariable<bool> EmbrionMega { get; private set; } = new();
        internal NetworkVariable<bool> ZapGunBattery { get; private set; } = new();
        internal NetworkVariable<bool> ButlerSquishy { get; private set; } = new();
        internal NetworkVariable<bool> GiantSquishy { get; private set; } = new();
        internal NetworkVariable<bool> JetpackUtility { get; private set; } = new();
        internal NetworkVariable<bool> AdamanceInteriors { get; private set; } = new();
        internal NetworkVariable<bool> DineMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> ProFlashlightPrice { get; private set; } = new();
        internal NetworkVariable<bool> VowMisty { get; private set; } = new();
        internal NetworkVariable<bool> NerfNightVision { get; private set; } = new();
        internal NetworkVariable<bool> StunLonger { get; private set; } = new();
        internal NetworkVariable<bool> ManeaterTarget { get; private set; } = new();
        internal NetworkVariable<bool> CadaverTarget { get; private set; } = new();
        internal NetworkVariable<bool> JetpackWarmUp { get; private set; } = new();
        internal NetworkVariable<bool> CruiserExhaust { get; private set; } = new();
        internal NetworkVariable<bool> CruiserRegen { get; private set; } = new();
        internal NetworkVariable<bool> CruiserTrees { get; private set; } = new();
        internal NetworkVariable<bool> CruiserEnemyDamage { get; private set; } = new();
        internal NetworkVariable<bool> CruiserCrashDamage { get; private set; } = new();
        internal NetworkVariable<bool> OffenseFireExits { get; private set; } = new();
        internal NetworkVariable<bool> DineFireExits { get; private set; } = new();
        internal NetworkVariable<bool> ProportionalFireExits { get; private set; } = new();
        internal NetworkVariable<bool> CruiserPatchEnemies { get; private set; } = new();
        internal NetworkVariable<bool> CruiserAutoHeal { get; private set; } = new();
        internal NetworkVariable<bool> CruiserDontStabilize { get; private set; } = new();
        internal NetworkVariable<bool> GirlBreakers { get; private set; } = new();
        internal NetworkVariable<int> RendSnowmen { get; private set; } = new();
        internal NetworkVariable<int> DineSnowmen { get; private set; } = new();
        internal NetworkVariable<int> TitanSnowmen { get; private set; } = new();
        internal NetworkVariable<int> CruiserPrice { get; private set; } = new();
        internal NetworkVariable<int> JetpackPrice { get; private set; } = new();
        internal NetworkVariable<int> JetpackControls { get; private set; } = new();
        internal NetworkVariable<int> WeedKillerDamage { get; private set; } = new();
        internal NetworkVariable<int> CruiserTurbos { get; private set; } = new();

        /*internal static void ConfigUpdated()
        {
            if (Instance != null)
                Instance.UpdateConfig();
        }*/

        void UpdateConfig()
        {
            if (!IsServer)
                return;

            // grab all values that should be server synced
            JesterWalkThrough.Value = Configuration.jesterWalkThrough.Value;
            JesterLongCooldown.Value = Configuration.jesterLongCooldown.Value;
            ButlerStealthStab.Value = Configuration.butlerStealthStab.Value;
            ButlerLongCooldown.Value = Configuration.butlerLongCooldown.Value;
            KnifeShortCooldown.Value = Configuration.knifeShortCooldown.Value;
            ManeaterLimitGrowth.Value = Configuration.maneaterLimitGrowth.Value;
            ManeaterWideTurns.Value = Configuration.maneaterWideTurns.Value;
            MoonsKillSwitch.Value = Configuration.moonsKillSwitch.Value;
            ExperimentationNoEvents.Value = Configuration.experimentationNoEvents.Value;
            VowMineshafts.Value = Configuration.vowMineshafts.Value;
            RendShrink.Value = Configuration.rendShrink.Value;
            DineFloods.Value = Configuration.dineFloods.Value;
            JetpackBattery.Value = Configuration.jetpackBattery.Value;
            ArtificeInteriors.Value = Configuration.artificeInteriors.Value;
            ZapGunPrice.Value = Configuration.zapGunPrice.Value;
            EmbrionMega.Value = Configuration.embrionMega.Value;
            ZapGunBattery.Value = Configuration.zapGunBattery.Value;
            ButlerSquishy.Value = Configuration.butlerSquishy.Value;
            RendSnowmen.Value = (int)Configuration.rendSnowmen.Value;
            DineSnowmen.Value = (int)Configuration.dineSnowmen.Value;
            TitanSnowmen.Value = (int)Configuration.titanSnowmen.Value;
            GiantSquishy.Value = Configuration.giantSquishy.Value;
            JetpackUtility.Value = Configuration.jetpackUtility.Value;
            AdamanceInteriors.Value = Configuration.adamanceInteriors.Value;
            DineMineshafts.Value = Configuration.dineMineshafts.Value;
            ProFlashlightPrice.Value = Configuration.proFlashlightPrice.Value;
            VowMisty.Value = Configuration.vowMisty.Value;
            NerfNightVision.Value = Configuration.nerfNightVision.Value;
            StunLonger.Value = Configuration.stunLonger.Value;
            CruiserPrice.Value = Configuration.cruiserPrice.Value;
            JetpackPrice.Value = Configuration.jetpackPrice.Value;
            ManeaterTarget.Value = Configuration.maneaterTarget.Value;
            CadaverTarget.Value = Configuration.cadaverTarget.Value;
            JetpackControls.Value = (int)Configuration.jetpackControls.Value;
            JetpackWarmUp.Value = Configuration.jetpackWarmUp.Value;
            CruiserExhaust.Value = Configuration.cruiserExhaust.Value;
            CruiserRegen.Value = Configuration.cruiserRegen.Value;
            CruiserTrees.Value = Configuration.cruiserTrees.Value;
            CruiserEnemyDamage.Value = Configuration.cruiserEnemyDamage.Value;
            CruiserCrashDamage.Value = Configuration.cruiserCrashDamage.Value;
            OffenseFireExits.Value = Configuration.offenseFireExits.Value;
            DineFireExits.Value = Configuration.dineFireExits.Value;
            ProportionalFireExits.Value = Configuration.proportionalFireExits.Value && !Common.INSTALLED_FAIRER_FIRE_EXITS;
            WeedKillerDamage.Value = Configuration.weedKillerDamage.Value;
            CruiserPatchEnemies.Value = Configuration.cruiserPatchEnemies.Value;
            CruiserTurbos.Value = Configuration.cruiserTurbos.Value;
            CruiserAutoHeal.Value = Configuration.cruiserAutoHeal.Value;
            CruiserDontStabilize.Value = !Configuration.cruiserStabilize.Value; // inverted, since it will default to false and VehicleController.Update() might check before sync
            GirlBreakers.Value = Configuration.girlBreakers.Value;

            OverrideCoordinator.ApplyOnServer();
            OverrideCoordinator.ApplyOnAllClients();
        }

        /*[Rpc(SendTo.ClientsAndHost)]
        internal void SyncScrapPriceRpc(NetworkObjectReference scrap, int value, bool node = true)
        {
            if (scrap.TryGet(out NetworkObject netObj) && netObj.TryGetComponent(out GrabbableObject item))
            {
                if (node)
                    item.SetScrapValue(value);
                else
                    item.scrapValue = value;
            }
            else
                Plugin.Logger.LogError("Failed to sync scrap price");
        }*/

        [Rpc(SendTo.ClientsAndHost)]
        internal void SyncFireExitRpc(NetworkObjectReference tele, int entranceId, bool isEntranceToBuilding = true, int audioReverbPreset = 2, bool fresh = true)
        {
            if (tele.TryGet(out NetworkObject netObj) && netObj.TryGetComponent(out EntranceTeleport entranceTeleport))
            {
                if (fresh)
                {
                    if (isEntranceToBuilding && !entranceTeleport.isEntranceToBuilding)
                    {
                        Common.extraFireExits.Add(entranceTeleport);
                        InteractTrigger interactTrigger = entranceTeleport.GetComponent<InteractTrigger>();
                        if (interactTrigger != null)
                        {
                            interactTrigger.hoverTip = interactTrigger.hoverTip.Replace("Exit", "Enter");
                            interactTrigger.timeToHold = 1.5f;
                        }
                    }
                }
                else
                {
                    if (entranceId == entranceTeleport.entranceId && isEntranceToBuilding == entranceTeleport.isEntranceToBuilding && audioReverbPreset == entranceTeleport.audioReverbPreset)
                        Plugin.Logger.LogDebug($"Fire exit \"{entranceTeleport.name}\" is already synced");
                    else if (entranceId != entranceTeleport.entranceId)
                        Plugin.Logger.LogWarning($"Fire exit #{entranceId} is currently ID {entranceTeleport.entranceId} on this client!!");

                    if (isEntranceToBuilding)
                    {
                        switch (entranceId)
                        {
                            case 1:
                                if (SceneOverrides.entranceTeleport1 == null)
                                    SceneOverrides.entranceTeleport1 = entranceTeleport;
                                break;
                            case 2:
                                if (SceneOverrides.entranceTeleport2 == null)
                                    SceneOverrides.entranceTeleport2 = entranceTeleport;
                                break;
                            case 3:
                                if (SceneOverrides.entranceTeleport3 == null)
                                    SceneOverrides.entranceTeleport3 = entranceTeleport;
                                break;
                        }
                    }
                }

                entranceTeleport.isEntranceToBuilding = isEntranceToBuilding;
                entranceTeleport.entranceId = entranceId;
                entranceTeleport.audioReverbPreset = audioReverbPreset;

                if (isEntranceToBuilding)
                {
                    Transform plane = entranceTeleport.transform.Find("Plane");
                    if (plane != null)
                        plane.gameObject.SetActive(false);
                }

                Plugin.Logger.LogDebug($"Synced {(fresh ? "new " : string.Empty)}fire exit \"{entranceTeleport.name}\" @ {entranceTeleport.entrancePoint.position} (ID: {entranceTeleport.entranceId}, Entrance: {entranceTeleport.isEntranceToBuilding})");
            }
            else
                Plugin.Logger.LogError($"Failed to sync entrance teleport #{entranceId}");
        }
    }
}
