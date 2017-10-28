using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Handshake : NetMessage
	{
		public uint secret;
		public uint playerId;
		public uint gameId;

		public Handshake() : base(Type.Handshake)
		{
		}

		public override void OnSerialize(Serializer s)
		{
			base.OnSerialize(s);
			this.secret = s.Serialize(secret);
			this.playerId = s.Serialize(playerId);
			this.gameId = s.Serialize(gameId);
		}
	}

	public class NetMessage : Serializable
	{
		public enum Type
		{
			Handshake
		}
		public Type MessageType;

		public NetMessage(Type type)
		{
			this.MessageType = type;
		}

		public override void OnSerialize(Serializer s)
		{
			this.MessageType = (Type)s.Serialize((int)this.MessageType);
		}
	}
}

