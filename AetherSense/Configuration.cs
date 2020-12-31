using AetherSense.Triggers;
using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace AetherSense
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public List<TriggerBase> Triggers { get; set; } = new List<TriggerBase>();

        public void Save()
        {
            Plugin.DalamudPluginInterface.SavePluginConfig(this);
        }
    }
}
