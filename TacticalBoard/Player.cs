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
		public enum ConnectionState
		{
			None,
			Connecting,
			Connected,
			Disconnnected
		}
		protected ConnectionState State = ConnectionState.Connecting;

		public bool IsConnected()
		{
			return (this.State == ConnectionState.Connected);
		}

		public Player(uint id, PlayerTeam team, ConnectionState state = ConnectionState.None)
		{
			this.Id = id;
			this.Team = team;
			this.State = state;
			Debug.Log("Create Player " + id + " team=" + team);
		}

		public uint Id;
		public uint GameId;
		public PlayerTeam Team;

		public uint [] Entities;
	}
}

