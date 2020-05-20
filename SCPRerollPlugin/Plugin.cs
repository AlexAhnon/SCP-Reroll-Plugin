using System;
using EXILED;

namespace SCPRerollPlugin
{
	public class Plugin : EXILED.Plugin
	{
		//Instance variable for eventhandlers
		public EventHandlers EventHandlers;
		
		public override void OnEnable()
		{
			try
			{
				// Set instance varible to a new instance, this should be nulled again in OnDisable
				EventHandlers = new EventHandlers(this);

				// Hook events to use
				Events.RoundStartEvent += EventHandlers.OnRoundStart;
				Events.RoundEndEvent += EventHandlers.OnRoundEnd;
				Events.ConsoleCommandEvent += EventHandlers.OnConsoleCommand;
				Log.Info("SCP Reroll plugin loaded.");
			}
			catch (Exception e)
			{
				Log.Error($"There was an error loading the plugin: {e}");
			}
		}

		public override void OnDisable()
		{
			Events.RoundStartEvent -= EventHandlers.OnRoundStart;
			Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
			Events.ConsoleCommandEvent -= EventHandlers.OnConsoleCommand;

			EventHandlers = null;
		}

		public override void OnReload()
		{
			//This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
		}

		public override string getName { get; } = "SCP Reroll Plugin";
	}
}