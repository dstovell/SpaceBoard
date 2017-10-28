using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetServer
	{
		private Hazel.Udp.UdpConnectionListener ConnListener;
		private Hazel.NetworkEndPoint EndPoint;

		private Dictionary<uint, NetServerGame> Games;
		private List<NetServerPlayer> PendingPlayers;

		public NetServer(int port, Hazel.IPMode ipMode = Hazel.IPMode.IPv4)
		{
			this.EndPoint = new Hazel.NetworkEndPoint(IPAddress.Any, port, ipMode);
			this.ConnListener = new Hazel.Udp.UdpConnectionListener(this.EndPoint);
			this.Games = new Dictionary<uint, NetServerGame>();

			this.AddListeners();
		}

		private void AddListeners()
		{
			this.ConnListener.NewConnection += this.OnNewConnection;
			//this.Conn.DataReceived += this.OnData;
			//this.Conn.Disconnected += this.OnDisconnect;
		}

		private void RemoveListeners()
		{
			this.ConnListener.NewConnection -= this.OnNewConnection;
			//this.Conn.DataReceived -= this.OnData;
		}

		private void OnNewConnection(object sender, Hazel.NewConnectionEventArgs a)
        {
        	Hazel.Connection newConn = a.Connection;

        	NetServerPlayer newPlayer = new NetServerPlayer(this, newConn);
        	this.PendingPlayers.Add(newPlayer);

            //Send the client some data
			//a.Connection.SendBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }, Hazel.SendOption.Reliable);

            //Disconnect from the client
            //a.Connection.Close();
        }

		public NetServerGame AddGame(uint gameId)
		{
			if (this.Games.ContainsKey(gameId))
			{
				return this.Games[gameId];
			}

			NetServerGame newGame = new NetServerGame(gameId);

			this.Games[gameId] = newGame;

			return newGame;
		}

		public void OnData(NetServerPlayer p, object obj, Hazel.DataReceivedEventArgs arg)
		{
			//This will be data about who the client is and what game they want

			uint playerId = 0;
			uint gameId = 0;

			p.Id = playerId;

			NetServerGame game = this.AddGame(gameId);
			game.AddPlayer(p);

			if (this.PendingPlayers.Contains(p))
			{
				this.PendingPlayers.Remove(p);
			}
		}

		public void OnDisconnect(NetServerPlayer p, object obj, Hazel.DisconnectedEventArgs arg)
		{
			if (this.PendingPlayers.Contains(p))
			{
				this.PendingPlayers.Remove(p);
			}
		}
	}
}
