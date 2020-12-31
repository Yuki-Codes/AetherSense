using System;
using System.Collections.Generic;
using Dalamud.Game.Command;

namespace AetherSense
{
	public static class CommandManagerExtensions
	{
		private static HashSet<string> addedHandlers = new HashSet<string>();

		public static void AddHandler(this CommandManager commandManager, string command, string helpText, Action<string> callback)
		{
			CommandInfo info = new CommandInfo((c, a) => callback(a));
			info.HelpMessage = helpText;
			info.ShowInHelp = helpText != null;
			commandManager.AddHandler(command, info);
			addedHandlers.Add(command);
		}

		public static void ClearHandlers(this CommandManager commandManager)
		{
			foreach (string command in addedHandlers)
			{
				commandManager.RemoveHandler(command);
			}
		}
	}
}
