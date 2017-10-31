using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetServerPlayer : Player
	{
		private NetServerGame Game;
		private NetServer Server;

		private Hazel.Connection Conn;

		public NetServerPlayer(uint playerId, NetServer server, Hazel.Connection conn) : base(playerId, PlayerTeam.Neutral, ConnectionState.Connecting)
		{
			this.Id = playerId;
			this.Server = server;
			this.Conn = conn;
			this.AddListeners();
		}

		public NetServerPlayer(uint playerId, Hazel.Connection conn) : base(playerId, PlayerTeam.Neutral, ConnectionState.Connecting)
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

		public void AssignToGame(uint gameId, PlayerTeam team, NetServerGame game)
		{
			this.GameId = gameId;
			this.Game = game;
			this.State = ConnectionState.Connected;
		}

		public void Send(byte [] data, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			this.Conn.SendBytes(data, sendOption);
		}

		public void Send(NetMessage msg, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			NetMessageHub.SendDataToConn(msg, this.Conn, sendOption);
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
			Debug.Log("Player " + this.Id + " got a disconnect");

			this.RemoveListeners();

			if (this.Game != null)
			{
				this.Game.OnDisconnect(this, obj, arg);
			}
			else if (this.Server != null)
			{
				this.Server.OnDisconnect(this, obj, arg);
			}
		}
	}
}