using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ButteRyBalance
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(GUID_LOBBY_COMPATIBILITY, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string PLUGIN_GUID = "butterystancakes.lethalcompany.butterybalance", PLUGIN_NAME = "ButteRyBalance", PLUGIN_VERSION = "0.2.3";
        internal static new ManualLogSource Logger;

        const string GUID_LOBBY_COMPATIBILITY = "BMX.LobbyCompatibility";
        const string GUID_ARTIFICE_BLIZZARD = "butterystancakes.lethalcompany.artificeblizzard";
        const string GUID_BARBER_FIXES = "butterystancakes.lethalcompany.barberfixes";
        const string GUID_VENT_SPAWN_FIX = "butterystancakes.lethalcompany.ventspawnfix";

        void Awake()
        {
            NetcodePatch();

            Logger = base.Logger;

            if (Chainloader.PluginInfos.ContainsKey(GUID_LOBBY_COMPATIBILITY))
            {
                Logger.LogInfo("CROSS-COMPATIBILITY - Lobby Compatibility detected");
                LobbyCompatibility.Init();
            }

            if (Chainloader.PluginInfos.ContainsKey(GUID_ARTIFICE_BLIZZARD))
            {
                Logger.LogInfo("CROSS-COMPATIBILITY - Artifice Blizzard detected");
                Common.INSTALLED_ARTIFICE_BLIZZARD = true;
            }

            if (Chainloader.PluginInfos.ContainsKey(GUID_BARBER_FIXES))
            {
                Logger.LogInfo("CROSS-COMPATIBILITY - Barber Fixes detected");
                Common.INSTALLED_BARBER_FIXES = true;
            }

            if (Chainloader.PluginInfos.ContainsKey(GUID_VENT_SPAWN_FIX))
            {
                Logger.LogInfo("CROSS-COMPATIBILITY - Vent Spawn Fix detected");
                Common.INSTALLED_VENT_SPAWN_FIX = true;
            }

            Configuration.Init(Config);

            new Harmony(PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }

        // https://github.com/EvaisaDev/UnityNetcodePatcher/blob/main/README.md
        void NetcodePatch()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}