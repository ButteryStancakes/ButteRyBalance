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
        }

        // --- NETWORKING ---

        void Start()
        {
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

        internal static void ConfigUpdated()
        {
            if (Instance != null)
                Instance.UpdateConfig();
        }

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
        }

        [ClientRpc]
        internal void SyncKnifePriceClientRpc(NetworkObjectReference knife, int value)
        {
            if (knife.TryGet(out NetworkObject netObj))
                netObj.GetComponent<GrabbableObject>().SetScrapValue(value);
            else
                Plugin.Logger.LogError("Failed to sync knife price from server");
        }
    }
}
