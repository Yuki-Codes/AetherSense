using Dalamud.Configuration;
using System;

namespace AetherSense
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public void Save()
        {
            Plugin.DalamudPluginInterface.SavePluginConfig(this);
        }
    }
}
