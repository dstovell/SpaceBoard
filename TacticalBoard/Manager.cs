using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public Game.GameActivityDel OnGameActivity;
		public Game.EntityActivityDel OnEntityActivity;

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

		public bool CreateGameObject(uint gameId = 0)
		{
			this.CurrentGame = new Game(gameId);
			this.CurrentGame.OnEntityActivity += this._OnEntityActivity;
			this.CurrentGame.OnGameActivity += this._OnGameActivity;
			return true;
		}

		public bool JoinGame(uint gameId, string ip, int port)
		{
			if (this.CurrentGame != null)
			{
				return false;
			}

			this.CreateGameObject(gameId);
			this.CurrentGame.Connect(ip, port);
			return true;
		}

		public bool FindGame(string ip, int port)
		{
			if (this.CurrentGame != null)
			{
				return false;
			}

			this.CreateGameObject();
			this.CurrentGame.Connect(ip, port);
			return true;
		}

		public Manager()
		{
		}

		private void _OnEntityActivity(List<EntityActivity> activity)
		{
			if (this.OnEntityActivity != null)
			{
				this.OnEntityActivity(activity);
			}
		}

		private void _OnGameActivity(Game.ActivityType type, Game game, Player player)
		{
			if (this.OnGameActivity != null)
			{
				this.OnGameActivity(type, game, player);
			}
		}
	}
}
