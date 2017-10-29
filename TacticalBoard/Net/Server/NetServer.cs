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
			this.PendingPlayers = new List<NetServerPlayer>();

			TacticalBoard.Manager.Init(10, 10, InterventionsManager.Flow.Server);

			this.AddListeners();
		}

		private void AddListeners()
		{
			this.ConnListener.NewConnection += this.OnNewConnection;
		}

		private void RemoveListeners()
		{
			this.ConnListener.NewConnection -= this.OnNewConnection;
		}

		private void OnNewConnection(object sender, Hazel.NewConnectionEventArgs a)
        {
			Debug.Log("sender=" + sender + " HandshakeData=" + a.HandshakeData.Length);
        	Hazel.Connection newConn = a.Connection;

        	int handShakeHeader = 3;

			if (a.HandshakeData.Length > handShakeHeader)
			{
	        	Handshake hs = new Handshake();
				NetMessageHub.DeSerializeData(hs, a.HandshakeData, handShakeHeader);
				Debug.Log("Handshake playerId=" + hs.playerId + " gameId=" + hs.gameId + " secret=" + hs.secret);

				NetServerPlayer newPlayer = new NetServerPlayer(this, newConn);

				if (hs.gameId != 0)
				{
					NetServerGame game = this.AddGame(hs.gameId);
					game.AddPlayer(newPlayer);
				}
				else
				{
					this.PendingPlayers.Add(newPlayer);
				}
			}
        }

		private void Start()
		{
			this.ConnListener.Start();
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

			NetMessageType type = NetMessage.GetType(arg.Bytes);

			switch (type)
			{
				case NetMessageType.Handshake:
				{
					break;
				}

				default:
				{
					break;
				}
			}
		}

		public void OnDisconnect(NetServerPlayer p, object obj, Hazel.DisconnectedEventArgs arg)
		{
			if (this.PendingPlayers.Contains(p))
			{
				this.PendingPlayers.Remove(p);
			}
		}

		public static int DefaultPort = 3334;
		public static string Usage = "mono TacticalBoardServer.exe <port>";

		static public void Main(string[] args)
	    {
//	    	if (args.Length < 1)
//	    	{
//				Debug.Log(Usage);
//				return;
//	    	}
//
//	    	int port = System.Int32.Parse(args[0]);
//	    	if (port <= 0)
//	    	{
//				Debug.Log(Usage);
//				return;
//	    	}

			int port = NetServer.DefaultPort;

	        Debug.Log("TacticalBoard Server will listen on port " + port);

			NetServer server = new NetServer(port);
			server.Start();

			Debug.Log("Press any key to exit");

			#if !UNITY_5
            System.Console.ReadKey();
            #endif
	    }
	}
}
