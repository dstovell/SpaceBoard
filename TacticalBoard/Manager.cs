using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public long TurnCount = 0;

		public Grid Board;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;

		uint EntityCount = 0;
		private Dictionary<uint, uint> EntityCounts;

		public static void Init(int x, int y, InterventionsManager.Flow flow = InterventionsManager.Flow.Local)
		{
			if (TacticalBoard.Manager.Instance == null)
			{
				TacticalBoard.Manager.Instance = new TacticalBoard.Manager(x, y);
				TacticalBoard.InterventionsManager.Init(flow);
			}
		}

		public void Update()
		{
			this.TurnCount++;

			InterventionsManager.Instance.Update(this.TurnCount);

			this.Board.UpdatePathfinding(this.Entites);

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				e.UpdateAssements(this.Entites);
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				e.UpdateMove();
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				e.UpdateAttack();
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

		public Player AddPlayer(uint id, PlayerTeam team, Hashtable parameters = null)
		{
			Player p = new Player(id, team, parameters);
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

		public Manager(int x, int y)
		{
			this.TurnCount = 0;
			this.EntityCounts = new Dictionary<uint,uint>();
			this.EntityCount = 0;

			this.Board = new SquareGrid(x, y);

			this.Players = new List<Player>();
			this.PlayerMap = new Dictionary<uint,Player>();

			this.Entites = new List<Entity>();
			this.EntityMap = new Dictionary<uint,Entity>();
		}
	}
}
