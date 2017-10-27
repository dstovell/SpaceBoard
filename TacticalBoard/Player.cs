using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public enum PlayerTeam
	{
		Neutral,
		TeamA,
		TeamB
	}

	public class Player
	{
		public Player(uint id, PlayerTeam team, Hashtable parameters = null)
		{
			this.Id = id;
			this.Team = team;
			this.Parameters = parameters;
		}

		public uint Id;
		public PlayerTeam Team;

		public Hashtable Parameters;
	}
}

