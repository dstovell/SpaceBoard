using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetServerPlayer
	{
		public enum ConnectionState
		{
			Connecting,
			Connected,
			Disconnnected
		}
		private ConnectionState State = ConnectionState.Connecting;

		public uint Id;
		public uint GameId;
		private NetServerGame Game;
		private NetServer Server;

		private Hazel.Connection Conn;

		public bool IsConnected()
		{
			return (this.State == ConnectionState.Connected);
		}

		public NetServerPlayer(NetServer server, Hazel.Connection conn)
		{
			this.Server = server;
			this.Conn = conn;
			this.AddListeners();
		}

		public NetServerPlayer(uint playerId, Hazel.Connection conn)
		{
			this.Id = playerId;
			this.Conn = conn;
			this.AddListeners();
		}

		private void AddListeners()
		{
			this.Conn.DataReceived += this.OnData;
			this.Conn.Disconnected += this.OnDisconnect;
		}

		private void RemoveListeners()
		{
			this.Conn.DataReceived -= this.OnData;
			this.Conn.Disconnected -= this.OnDisconnect;
		}

		public void AssignToGame(uint gameId, NetServerGame game)
		{
			this.GameId = gameId;
			this.Game = game;
			this.State = ConnectionState.Connected;
		}

		public void Send()
		{
		}

		private void OnData(object obj, Hazel.DataReceivedEventArgs arg)
		{
			if (!this.IsConnected())
			{
				if (this.Server != null)
				{
					this.Server.OnData(this, obj, arg);
				}
				return;
			}

			if (this.Game != null)
			{
				this.Game.OnData(this, obj, arg);
			}
		}

		private void OnDisconnect(object obj, Hazel.DisconnectedEventArgs arg)
		{
			if (!this.IsConnected())
			{
				if (this.Server != null)
				{
					this.Server.OnDisconnect(this, obj, arg);
				}
				return;
			}

			if (this.Game != null)
			{
				this.Game.OnDisconnect(this, obj, arg);
			}
		}
	}

	public class NetServerGame
	{
		public uint Id;

		private List<NetServerPlayer> Players;

		public NetServerGame(uint id)
		{
			this.Id = id;
			this.Players = new List<NetServerPlayer>();	
		}

		public void AddPlayer(uint playerId, Hazel.Connection conn)
		{
			NetServerPlayer player = new NetServerPlayer(playerId, conn);
			this.AddPlayer(player);
		}

		public void AddPlayer(NetServerPlayer p)
		{
			if (this.Players.Contains(p))
			{
				return;
			}

			this.Players.Add(p);

			p.AssignToGame(this.Id, this);
		}

		public void SendToPlayers()
		{
			//Do we need to make this thread safe?
			for (int i=0; i<this.Players.Count; i++)
			{
				NetServerPlayer p = this.Players[i];
				if (p.IsConnected())
				{
					p.Send();
				}
			}
		}

		public void OnData(NetServerPlayer p, object obj, Hazel.DataReceivedEventArgs arg)
		{
			//For now, just rebroadcast
			this.SendToPlayers();
		}

		public void OnDisconnect(NetServerPlayer p, object obj, Hazel.DisconnectedEventArgs arg)
		{
			//For now, just rebroadcast
			this.SendToPlayers();
			this.Players.Remove(p);
		}
	}
}


