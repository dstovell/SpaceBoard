using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Game
	{
		public static uint MaxPlayers = 1;

		public enum SimType
		{
			Local,
			Remote
		}
		protected SimType Sim;

		public enum GameState
		{
			None,
			WaitingToConnect,
			WaitingForPlayers,
			WaitingForStart,
			Running,
			Ended
		}
		protected GameState State = GameState.None;

		public bool IsRunning() 
		{ 
			return (this.State == GameState.Running);
		}

		public Game(uint gameId = 0)
		{
			this.Id = gameId;
			this.Sim = SimType.Local;

			this.TurnCount = 0;
			this.EntityCounts = new Dictionary<uint,uint>();

			this.Players = new List<Player>();
			this.PlayerMap = new Dictionary<uint,Player>();

			this.Entites = new List<Entity>();
			this.EntityMap = new Dictionary<uint,Entity>();
		}

		protected long deltaTime = 0;
		public virtual long GetTime()
		{
			return ((System.DateTime.UtcNow.Ticks/System.TimeSpan.TicksPerMillisecond) + this.deltaTime);
		}

		public long SetDeltaTime(long serverTime, long latency = 0)
		{
			this.deltaTime = (serverTime - (System.DateTime.UtcNow.Ticks/System.TimeSpan.TicksPerMillisecond));
			return this.GetTime(); 
		}

		public void Connect(string ip, int port)
		{
			if (ip == null)
			{
				ip = "127.0.0.1";
			}

			this.Sim = SimType.Remote;
			this.State = GameState.WaitingToConnect;
			this.Client = new NetClient(this, ip, NetServer.DefaultPort);
			this.Client.Connect();
		}

		public void LoadLevel(uint level)
		{
			LevelParams lp = Data.Levels.ContainsKey(level) ? Data.Levels[level] : null;
			if (lp != null)
			{
				Debug.Log("LoadLevel " + lp.StringId + " " + level);
				this.LevelId = level;
				this.CreateBoard(lp);
			}
		}

		public void LoadRandomLevel()
		{
			string levelName = "Test01";
			uint levelId = Data.GetHash(levelName);
			LevelParams lp = Data.Levels.ContainsKey(levelId) ? Data.Levels[levelId] : null;
			if (lp != null)
			{
				Debug.Log("LoadRandomLevel " + levelName + " " + lp.Id + " size=" + lp.SizeX + "," + lp.SizeY);
				this.LoadLevel(levelId);
			}
		}

		public void CreateBoard(LevelParams lp)
		{
			this.Board = new SquareGrid(lp.SizeX, lp.SizeY);
		}

		public void AddPlayer(Player p)
		{
			if (this.Players.Contains(p))
			{
				return;
			}

			this.Players.Add(p);
		}

		public uint RemainingPlayerSlots()
		{
			return (uint)System.Math.Max(0, (Game.MaxPlayers - (uint)this.Players.Count));
		}

		public bool IsFull()
		{
			return (this.RemainingPlayerSlots() == 0);
		}

		public uint Id;
		public long StartTime = 0;
		public long EndTime = 0;
		public long TurnCount = 0;
		public uint LevelId;
		public Grid Board;

		protected long LastTurnUpdate = 0;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;

		private Dictionary<uint, uint> EntityCounts;

		private InterventionsManager Interventions;
		private NetClient Client;

		public virtual void Update()
		{
			long now = this.GetTime();
			bool hasStarted = ((this.StartTime != 0) && (this.StartTime <= now));
			bool hasEnded = ((this.EndTime != 0) && (this.EndTime > now));
			bool inTimeFrame = (hasStarted && !hasEnded);

			if (this.State == GameState.Ended)
			{
				return;
			}

			if (!this.IsRunning())
			{
				if (inTimeFrame)
				{
					Debug.Log("Running " + this.GetTime());
					this.State  = GameState.Running;
				}
				else
				{
					return;
				}
			}

			if (this.IsRunning())
			{
				long timeSinceLastUpdate = now - this.LastTurnUpdate;

				if (timeSinceLastUpdate > Data.MillisecondsPerTurn)
				{
					UpdateTurn();
					this.LastTurnUpdate = (this.LastTurnUpdate != 0) ? (this.LastTurnUpdate + Data.MillisecondsPerTurn) : now;
					//Debug.Log("TurnCount=" + this.TurnCount + " " + this.Entites.Count + " time=" + this.GetTime());
				}
			}
		}

		private void UpdateTurn()
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
			if (this.PlayerMap.ContainsKey(id))
			{
				return this.PlayerMap[id];
			}

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

		public void HandleGameJoined(GameJoined msg)
		{
			this.Id = msg.GameId;
			this.State = GameState.WaitingForPlayers;

			this.SetDeltaTime(msg.ServerTime);

			this.LoadLevel(msg.LevelId);
		}

		public void HandleGameStart(GameStart msg)
		{
			if (this.State == GameState.WaitingForPlayers)
			{
				this.StartTime = msg.AtTime;
				this.State = GameState.WaitingForStart;
				Debug.Log("Starting Game at: " + this.StartTime);
			}
		}

		public void HandleGameEnd(GameEnd msg)
		{
			
		}

		public void HandlePlayerJoin(PlayerJoin msg)
		{
			this.AddPlayer(msg.PlayerId, PlayerTeam.Neutral);
		}

		public void HandlePlayerLeave(PlayerLeave msg)
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
