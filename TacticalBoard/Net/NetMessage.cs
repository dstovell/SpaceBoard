using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public enum NetMessageType
	{
		Handshake,
		GameJoined,
		GameStart,
		GameEnd,
		PlayerJoin,
		PlayerLeave,
		PlayerIntervention,
		Unknown
	}

	public class NetMessage : Serializable
	{
		public NetMessageType MessageType;

		public NetMessage(NetMessageType type)
		{
			this.MessageType = type;
		}

		public override void OnSerialize(Serializer s)
		{
			this.MessageType = (NetMessageType)s.Serialize((int)this.MessageType);
		}

		public static NetMessageType GetType(byte [] buffer, int msgStart = 0)
		{
			return (NetMessageType)System.BitConverter.ToInt32(buffer, msgStart);
		}
	}

	public class Handshake : NetMessage
	{
		public static uint _secret = 42;

		public uint secret;
		public uint playerId;
		public uint [] entites;

		public Handshake() : base(NetMessageType.Handshake)
		{
			this.secret = Handshake._secret;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.secret = s.Serialize(this.secret);
			this.playerId = s.Serialize(this.playerId);
			this.entites = s.Serialize(this.entites);
		}
	}

	public class GameJoined : NetMessage
	{
		public uint GameId;
		public uint PlayerId;
		public PlayerTeam Team;
		public long ServerTime;
		public uint LevelId;
		public uint [] Entities;

		public GameJoined() : base(NetMessageType.GameJoined)
		{
		}

		public GameJoined(uint id, uint playerId, uint [] entities, uint levelId, PlayerTeam team, long serverTime) : base(NetMessageType.GameJoined)
		{
			this.GameId = id;
			this.PlayerId = playerId;
			this.LevelId = levelId;
			this.Team = team;
			this.ServerTime = serverTime;
			this.Entities = entities;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.GameId = s.Serialize(this.GameId);
			this.PlayerId = s.Serialize(this.PlayerId);
			this.Team = (PlayerTeam)s.Serialize((int)this.Team);
			this.ServerTime = s.Serialize(this.ServerTime);
			this.LevelId = s.Serialize(this.LevelId);
			this.Entities = s.Serialize(this.Entities);
		}
	}

	public class PlayerJoin : NetMessage
	{
		public uint PlayerId;
		public PlayerTeam Team;
		public uint [] Entities;

		public PlayerJoin() : base(NetMessageType.PlayerJoin)
		{
		}

		public PlayerJoin(uint id, PlayerTeam team, uint [] entities) : base(NetMessageType.PlayerJoin)
		{
			this.PlayerId = id;
			this.Team = team;
			this.Entities = entities;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.PlayerId = s.Serialize(this.PlayerId);
			this.Team = (PlayerTeam)s.Serialize((int)this.Team);
			this.Entities = s.Serialize(this.Entities);
		}
	}

	public class PlayerLeave : NetMessage
	{
		public uint PlayerId;

		public PlayerLeave() : base(NetMessageType.PlayerLeave)
		{
		}

		public PlayerLeave(uint id) : base(NetMessageType.PlayerLeave)
		{
			this.PlayerId = id;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.PlayerId = s.Serialize(this.PlayerId);
		}
	}

	public class GameStart : NetMessage
	{
		public long AtTime;

		public GameStart() : base(NetMessageType.GameStart)
		{
		}

		public GameStart(long serverTime) : base(NetMessageType.GameStart)
		{
			this.AtTime = serverTime;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.AtTime = s.Serialize(this.AtTime);
		}
	}

	public class GameEnd : NetMessage
	{
		public long AtTime;
		public PlayerTeam WinningTeam;

		public GameEnd() : base(NetMessageType.GameEnd)
		{
		}

		public GameEnd(long serverTime, PlayerTeam winningTeam) : base(NetMessageType.GameEnd)
		{
			this.AtTime = serverTime;
			this.WinningTeam = winningTeam;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.AtTime = s.Serialize(this.AtTime);
			this.WinningTeam = (PlayerTeam)s.Serialize((int)this.WinningTeam);
		}
	}

	public class PlayerIntervention : NetMessage
	{
		public Request InterventionRequest;

		public PlayerIntervention(Request playerRequest) : base(NetMessageType.PlayerIntervention)
		{
			this.InterventionRequest = playerRequest;
		}

		public PlayerIntervention() : base(NetMessageType.PlayerIntervention)
		{
			this.InterventionRequest = new Request();
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.InterventionRequest.OnSerialize(s);
		}
	}
}

