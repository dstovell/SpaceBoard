using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetServer
	{
		public static uint MaxGames = 100;

		private Hazel.Udp.UdpConnectionListener ConnListener;
		private Hazel.NetworkEndPoint EndPoint;

		private Dictionary<uint, NetServerGame> Games;
		private List<NetServerPlayer> PendingPlayers;

		private uint LastGameId = 100;

		public NetServer(int port, Hazel.IPMode ipMode = Hazel.IPMode.IPv4)
		{
			this.EndPoint = new Hazel.NetworkEndPoint(IPAddress.Any, port, ipMode);
			this.ConnListener = new Hazel.Udp.UdpConnectionListener(this.EndPoint);
			this.Games = new Dictionary<uint, NetServerGame>();
			this.PendingPlayers = new List<NetServerPlayer>();

			TacticalBoard.Data.Init();

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

			if (a.HandshakeData.Length > 0)
			{
	        	Handshake hs = new Handshake();
				NetMessageHub.DeSerializeData(hs, a.HandshakeData);
				Debug.Log("Handshake playerId=" + hs.playerId + " secret=" + hs.secret);

				NetServerPlayer newPlayer = new NetServerPlayer(hs.playerId, this, newConn);

				NetServerGame game = this.MatchmakeFromExisting(newPlayer);

				if (game == null)
				{
					this.PendingPlayers.Add(newPlayer);
					return;
				}
			}
        }

		private void Start()
		{
			this.ConnListener.Start();
		}

		public NetServerGame MatchmakeFromExisting(NetServerPlayer p)
		{
			NetServerGame game = null;

			//For now we will just find a game or add one!
			foreach(KeyValuePair<uint,NetServerGame> pair in this.Games)
			{
				if (!pair.Value.IsFull())
				{
					game = pair.Value;
				}
			}

			if (game == null)
			{
				if (this.Games.Count >= NetServer.MaxGames)
				{
					return null;
				}

				game = this.AddGame();
			}

			game.AddPlayer(p);

			return game;
		}

		public NetServerGame AddGame(uint gameId = 0)
		{
			if (gameId == 0)
			{
				gameId = this.LastGameId;
				this.LastGameId++;
			}

			if (this.Games.ContainsKey(gameId))
			{
				return this.Games[gameId];
			}

			NetServerGame newGame = new NetServerGame(gameId);

			this.Games[gameId] = newGame;

			return newGame;
		}

		public void Update()
		{
			foreach (KeyValuePair<uint,NetServerGame> pair in this.Games)
			{
				pair.Value.Update();
			}
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

			Debug.Log("Press Ctrl+C to exit");
			while (true)
			{
				server.Update();
				System.Threading.Thread.Sleep(10);
			}
	    }
	}
}
