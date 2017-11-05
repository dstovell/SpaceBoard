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

		public bool SetState(ConnectionState s)
		{
			this.State = s;
			return true;
		}

		public Player(uint id, PlayerTeam team, ConnectionState state = ConnectionState.None)
		{
			this.Id = id;
			this.Team = team;
			this.State = state;
		}

		public uint Id;
		public uint GameId;
		public PlayerTeam Team;

		public uint [] Entities;
	}
}

