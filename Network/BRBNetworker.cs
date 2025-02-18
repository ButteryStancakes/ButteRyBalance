using ButteRyBalance.Overrides;
using GameNetcodeStuff;
using System.Reflection;
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
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Assembly.GetCallingAssembly().GetName().Name + prefab.name));
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
                    Instantiate(prefab).GetComponent<NetworkObject>().Spawn();
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
        internal NetworkVariable<bool> FoggyLimit { get; private set; } = new();
        internal NetworkVariable<bool> ExperimentationNoEvents { get; private set; } = new();
        internal NetworkVariable<bool> RandomIndoorFog { get; private set; } = new();
        internal NetworkVariable<bool> VowMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> ShrinkMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> OffenseMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> MarchShrink { get; private set; } = new();
        internal NetworkVariable<bool> MarchRainy { get; private set; } = new();
        internal NetworkVariable<bool> MultiplayerWeather { get; private set; } = new();
        internal NetworkVariable<bool> RendShrink { get; private set; } = new();
        internal NetworkVariable<bool> RendMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> TitanMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> DineFloods { get; private set; } = new();
        internal NetworkVariable<bool> NutcrackerGunPrice { get; private set; } = new();
        internal NetworkVariable<bool> JetpackBattery { get; private set; } = new();
        internal NetworkVariable<bool> JetpackReduceDiscount { get; private set; } = new();
        internal NetworkVariable<bool> TZPExpandCapacity { get; private set; } = new();
        internal NetworkVariable<bool> JetpackInertia { get; private set; } = new();
        internal NetworkVariable<bool> ArtificeInteriors { get; private set; } = new();
        internal NetworkVariable<bool> ZapGunPrice { get; private set; } = new();
        internal NetworkVariable<bool> RadarBoosterPrice { get; private set; } = new();
        internal NetworkVariable<bool> StunGrenadePrice { get; private set; } = new();
        internal NetworkVariable<bool> ScrapAdjustWeights { get; private set; } = new();
        internal NetworkVariable<bool> EmbrionMineshafts { get; private set; } = new();
        internal NetworkVariable<bool> EmbrionMega { get; private set; } = new();
        internal NetworkVariable<bool> ZapGunBattery { get; private set; } = new();
        internal NetworkVariable<bool> ApparatusPrice { get; private set; } = new();
        internal NetworkVariable<bool> RobotRider { get; private set; } = new();

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
            FoggyLimit.Value = Configuration.foggyLimit.Value;
            ExperimentationNoEvents.Value = Configuration.experimentationNoEvents.Value;
            RandomIndoorFog.Value = Configuration.randomIndoorFog.Value;
            VowMineshafts.Value = Configuration.vowMineshafts.Value;
            ShrinkMineshafts.Value = Configuration.shrinkMineshafts.Value;
            OffenseMineshafts.Value = Configuration.offenseMineshafts.Value;
            MarchShrink.Value = Configuration.marchShrink.Value;
            MarchRainy.Value = Configuration.marchRainy.Value;
            MultiplayerWeather.Value = Configuration.multiplayerWeather.Value;
            RendShrink.Value = Configuration.rendShrink.Value;
            RendMineshafts.Value = Configuration.rendMineshafts.Value;
            TitanMineshafts.Value = Configuration.titanMineshafts.Value;
            DineFloods.Value = Configuration.dineFloods.Value;
            NutcrackerGunPrice.Value = Configuration.nutcrackerGunPrice.Value;
            JetpackBattery.Value = Configuration.jetpackBattery.Value;
            JetpackReduceDiscount.Value = Configuration.jetpackReduceDiscount.Value;
            TZPExpandCapacity.Value = Configuration.tzpExpandCapacity.Value;
            JetpackInertia.Value = Configuration.jetpackInertia.Value;
            ArtificeInteriors.Value = Configuration.artificeInteriors.Value;
            ZapGunPrice.Value = Configuration.zapGunPrice.Value;
            RadarBoosterPrice.Value = Configuration.radarBoosterPrice.Value;
            StunGrenadePrice.Value = Configuration.stunGrenadePrice.Value;
            ScrapAdjustWeights.Value = Configuration.scrapAdjustWeights.Value;
            EmbrionMineshafts.Value = Configuration.embrionMineshafts.Value;
            EmbrionMega.Value = Configuration.embrionMega.Value;
            ZapGunBattery.Value = Configuration.zapGunBattery.Value;
            ApparatusPrice.Value = Configuration.apparatusPrice.Value;
            RobotRider.Value = Configuration.robotRider.Value;

            OverrideCoordinator.ApplyOnServer();
            OverrideCoordinator.ApplyOnAllClients();
        }

        [ClientRpc]
        internal void SyncScrapPriceClientRpc(NetworkObjectReference scrap, int value)
        {
            if (scrap.TryGet(out NetworkObject netObj))
                netObj.GetComponent<GrabbableObject>().SetScrapValue(value);
            else
                Plugin.Logger.LogError("Failed to sync scrap price from server");
        }

        [ServerRpc]
        internal void SyncScrapPriceServerRpc(NetworkObjectReference scrap, int value)
        {
            SyncScrapPriceClientRpc(scrap, value);
        }

        [ServerRpc]
        internal void SyncCrouchingServerRpc(int playerID, bool crouching)
        {
            SyncCrouchingClientRpc(playerID, crouching);
        }

        [ClientRpc]
        internal void SyncCrouchingClientRpc(int playerID, bool crouching)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];
            if (player.IsOwner)
                return;

            player.isCrouching = crouching;
        }
    }
}
