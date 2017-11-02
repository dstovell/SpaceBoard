using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public long TurnCount
		{
			get
			{
				return (this.CurrentGame != null) ? this.CurrentGame.TurnCount : 0;
			}
		}

		public Game CurrentGame;

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
