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

		public GameJoined() : base(NetMessageType.GameJoined)
		{
		}

		public GameJoined(uint id, uint playerId, uint levelId, PlayerTeam team, long serverTime) : base(NetMessageType.GameJoined)
		{
			this.GameId = id;
			this.PlayerId = playerId;
			this.LevelId = levelId;
			this.Team = team;
			this.ServerTime = serverTime;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.GameId = s.Serialize(this.GameId);
			this.PlayerId = s.Serialize(this.PlayerId);
			this.Team = (PlayerTeam)s.Serialize((int)this.Team);
			this.ServerTime = s.Serialize(this.ServerTime);
			this.LevelId = s.Serialize(this.LevelId);
		}
	}

	public class PlayerJoin : NetMessage
	{
		public uint PlayerId;

		public PlayerJoin() : base(NetMessageType.PlayerJoin)
		{
		}

		public PlayerJoin(uint id) : base(NetMessageType.PlayerJoin)
		{
			this.PlayerId = id;
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.PlayerId = s.Serialize(this.PlayerId);
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
		public uint RequestId;
		public uint PlayerId;
		public long TurnRequested;
		public long Turn;
		public uint EntityId;
		public ushort GridNodeId;
		public InterventionType Type;

		public ResultType Result;
		public bool Delivered;

		public PlayerIntervention(Request playerRequest) : base(NetMessageType.PlayerIntervention)
		{
			this.RequestId = playerRequest.RequestId;
			this.PlayerId = playerRequest.PlayerId;
			this.TurnRequested = playerRequest.TurnRequested;
			this.Turn = playerRequest.Turn;
			this.EntityId = playerRequest.EntityId;
			this.GridNodeId = playerRequest.GridNodeId;

			this.Type = playerRequest.Type;
			this.Result = playerRequest.Result;
		}

		public PlayerIntervention() : base(NetMessageType.PlayerIntervention)
		{
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.RequestId = s.Serialize(this.RequestId);
			this.PlayerId = s.Serialize(this.PlayerId);
			this.TurnRequested = s.Serialize(this.TurnRequested);
			this.Turn = s.Serialize(this.Turn);
			this.EntityId = s.Serialize(this.EntityId);
			this.GridNodeId = s.Serialize(this.GridNodeId);

			this.Type = (InterventionType)s.Serialize((int)this.Type);
			this.Result = (ResultType)s.Serialize((int)this.Result);
		}
	}
}

