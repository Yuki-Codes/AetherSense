using System;
using System.IO;
using System.Reflection;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace AetherSense
{
	public static class DalamudPluginInterfaceExtensions
	{
		public static string GetPluginDirectory(this DalamudPluginInterface self)
		{
			if (self == null)
				return ".";

			Type pluginInterfaceType = typeof(DalamudPluginInterface);
			FieldInfo configsField = pluginInterfaceType.GetField("configs", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo pluginNameField = pluginInterfaceType.GetField("pluginName", BindingFlags.Instance | BindingFlags.NonPublic);

			PluginConfigurations configs = (PluginConfigurations)configsField.GetValue(Plugin.DalamudPluginInterface);
			string pluginName = (string)pluginNameField.GetValue(self);

			Type configsType = typeof(PluginConfigurations);
			FieldInfo configDirectoryField = configsType.GetField("configDirectory", BindingFlags.Instance | BindingFlags.NonPublic);
			DirectoryInfo configDirectory = (DirectoryInfo)configDirectoryField.GetValue(configs);

			string dir = Path.Combine(configDirectory.FullName, "..");

			string pluginDir = Path.Combine(dir, "devPlugins", pluginName);
			if (!Directory.Exists(pluginDir))
			{
				pluginDir = Path.Combine(dir, "installedPlugins", pluginName);

				if (!Directory.Exists(pluginDir))
				{
					throw new Exception("Failed to locate plugin parent directory");
				}
			}

			string version = typeof(Plugin).Assembly.GetName().Version.ToString();

			if (File.Exists(Path.Combine(pluginDir, pluginName + ".dll")))
				return pluginDir;

			pluginDir = Path.Combine(pluginDir, version);

			if (!Directory.Exists(pluginDir))
				throw new Exception($"Failed to locate plugin version directory: {version}");

			return pluginDir;
		}
	}
}
