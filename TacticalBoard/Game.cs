using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Game
	{
		public enum GameState
		{
			None,
			WaitingForPlayers,
			WaitingForStart,
			Running,
			Ended
		}
		protected GameState State = GameState.None;

		public Game(uint gameId)
		{
			this.Id = gameId;

			this.TurnCount = 0;
			this.EntityCounts = new Dictionary<uint,uint>();

			this.Players = new List<Player>();
			this.PlayerMap = new Dictionary<uint,Player>();

			this.Entites = new List<Entity>();
			this.EntityMap = new Dictionary<uint,Entity>();
		}

		public void Connect(string ip, int port)
		{
			this.Client = new NetClient(this, "127.0.0.1", NetServer.DefaultPort);
			this.Client.Connect();
		}

		public void CreateBoard()
		{
			
		}

		public void AddPlayer(Player p)
		{
			if (this.Players.Contains(p))
			{
				return;
			}

			this.Players.Add(p);
		}

		public uint Id;
		public long TurnCount = 0;
		public Grid Board;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;

		private Dictionary<uint, uint> EntityCounts;

		private InterventionsManager Interventions;
		private NetClient Client;

		public void Update()
		{
			this.TurnCount++;

			if (this.Interventions != null)
			{
				this.Interventions.Update(this.TurnCount);
			}

			if (this.Board != null)
			{
				this.Board.UpdatePathfinding(this.Entites);
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				if (e.IsActive())
				{
					e.UpdateAssements(this.Entites);
				}
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				if (e.IsActive())
				{
					e.UpdateMove();
				}
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				if (e.IsActive())
				{
					e.UpdateAttack();
				}
			}
		}

		public uint GenerateEntityId(uint playerId)
		{
			this.EntityCounts[playerId]++;
			uint newId = this.EntityCounts[playerId];

			string hashableString = string.Format("{0}_{1}", playerId, newId);
			return (uint)hashableString.GetHashCode();
		}

		public Entity AddEntity(PlayerTeam team, uint playerId, EntityParams ep, Brain br = null)
		{
			uint newId = this.GenerateEntityId(playerId);

			Entity e = new Entity(newId, team, playerId, this.Board, ep, br);
			this.Entites.Add(e);
			this.Entites.Sort(this.EntityComparer);
			this.EntityMap[newId] = e;
			return e;
		}

		public Player AddPlayer(uint id, PlayerTeam team)
		{
			Player p = new Player(id, team);
			this.Players.Add(p);
			this.Players.Sort(this.PlayerComparer);
			this.PlayerMap[id] = p;
			this.EntityCounts[id] = 0;
			return p;
		}

		public Player GetPlayer(uint id)
		{
			return this.PlayerMap.ContainsKey(id) ? this.PlayerMap[id] : null;
		}

		public void HandleSetupBoard()
		{
			//this.Board = new SquareGrid(x, y);
		}

		public void HandlePlayerJoin()
		{
		}

		public void HandlePlayerLeave()
		{
		}

		private int EntityComparer(Entity a, Entity b)
		{
			//Should this be the other way?
			return ((int)a.Id - (int)b.Id);
		}

		private int PlayerComparer(Player a, Player b)
		{
			//Should this be the other way?
			return ((int)a.Id - (int)b.Id);
		}
	}
}
