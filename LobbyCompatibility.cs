﻿using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace ButteRyBalance
{
    internal static class LobbyCompatibility
    {
        internal static void Init()
        {
            PluginHelper.RegisterPlugin(Plugin.PLUGIN_GUID, System.Version.Parse(Plugin.PLUGIN_VERSION), CompatibilityLevel.Everyone, VersionStrictness.Patch);
        }
    }
}
