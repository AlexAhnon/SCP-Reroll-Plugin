using System.Collections.Generic;
using EXILED;
using EXILED.Extensions;
using MEC;

namespace SCPRerollPlugin
{
	public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		private bool canRoll = false;
		private Dictionary<ReferenceHub, bool> playerCanRoll = new Dictionary<ReferenceHub, bool>();

		public void OnRoundStart()
		{
			foreach (ReferenceHub hub in Player.GetHubs())
			{
				//Since this event fires before everyone has initially spawned, you need to wait before doing things like changing their health, adding items, etc
				Timing.RunCoroutine(WaitForPlayerSpawns(hub));
			}

			Timing.CallDelayed(13f, () =>
			{
				// Set ability to roll to false
				canRoll = false;
			});
		}

		public void OnConsoleCommand(ConsoleCommandEvent ev)
		{
			string cmd = ev.Command.ToLower();
			if (cmd.StartsWith("scproll"))
			{
				if (canRoll && playerCanRoll[ev.Player] == true)
				{
					int val;
					val = RollSCP(ev.Player);
					playerCanRoll[ev.Player] = false;

					if (val == 0)
					{
						ev.ReturnMessage = "You are now " + ev.Player.GetRole().ToString();
					} else
					{
						ev.ReturnMessage = "There are no available SCPs to swap into.";
					}
					return;
				} else
				{
					ev.ReturnMessage = "You do not have permission to roll.";
					return;
				}
			}
		}

		public IEnumerator<float> WaitForPlayerSpawns(ReferenceHub hub)
		{
			// Wait 2 seconds to make sure everyone is spawned in correctly
			yield return Timing.WaitForSeconds(2f);

			// Set ability to roll to true
			canRoll = true;

			// Broadcast ability to roll SCP
			if (hub.GetTeam() == Team.SCP)
			{
				playerCanRoll.Add(hub, true);
				hub.Broadcast(8, "You have 10 seconds to type .scproll in the client console if you wish to roll for another SCP");
			}
		}

		// Roll for a random SCP excluding the one you are
		public int RollSCP(ReferenceHub hub)
		{
			List<RoleType> SCPRoles = new List<RoleType>();

			SCPRoles.Add((RoleType)0);
			SCPRoles.Add((RoleType)3);
			SCPRoles.Add((RoleType)5);
			SCPRoles.Add((RoleType)7);
			SCPRoles.Add((RoleType)9);
			SCPRoles.Add((RoleType)16);
			SCPRoles.Add((RoleType)17);

			foreach (KeyValuePair<ReferenceHub, bool> entry in playerCanRoll)
			{
				SCPRoles.Remove(entry.Key.GetRole());
			}

			if (playerCanRoll.Count <= 1)
			{
				SCPRoles.Remove((RoleType)7);
			}

			if (playerCanRoll.Count == 0)
			{
				return 1;
			}

			System.Random random = new System.Random();

			int ranValue = random.Next(SCPRoles.Count);

			hub.SetRole(SCPRoles[ranValue], false);

			return 0;
		}

		public void OnRoundEnd()
		{
			playerCanRoll = new Dictionary<ReferenceHub, bool>();
		}
	}
}