using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public long TurnCount = 0;

		public Grid Board;

		public Game CurrentGame;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;

		private Dictionary<uint, uint> EntityCounts;

		private NetClient Client;

		public static void Init()
		{
			if (TacticalBoard.Manager.Instance == null)
			{
				TacticalBoard.Manager.Instance = new TacticalBoard.Manager();
				TacticalBoard.Data.Init();
			}
		}

		public void Update()
		{
			if (this.CurrentGame != null)
			{
				this.CurrentGame.Update();
			}
		}

		public bool JoinGame(uint gameId, string ip, int port)
		{
			if (this.CurrentGame != null)
			{
				return false;
			}

			this.CurrentGame = new Game(gameId);
			this.CurrentGame.Connect(ip, port);
			return true;
		}

		public bool FindGame(string ip, int port)
		{
			if (this.CurrentGame != null)
			{
				return false;
			}

			this.CurrentGame = new Game();
			this.CurrentGame.Connect(ip, port);
			return true;
		}

		public Manager()
		{
		}
	}
}
