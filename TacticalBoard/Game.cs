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

		public void LoadLevel(uint levelId)
		{
			LevelParams lp = Data.GetLevelData(levelId);
			if (lp != null)
			{
				Debug.Log("LoadLevel " + lp.StringId + " " + levelId);
				this.LevelId = levelId;
				this.CreateBoard(lp);
			}
		}

		public void LoadRandomLevel()
		{
			string levelName = "Test01";
			uint levelId = Data.GetHash(levelName);
			LevelParams lp = Data.GetLevelData(levelId);
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
		public long StartTime { get; protected set; }
		public long EndTime { get; protected set; }
		public long TurnCount { get; protected set; }
		public uint LevelId	{ get; protected set; }
		public Grid Board;

		protected long LastTurnUpdate = 0;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;

		protected Dictionary<uint, uint> EntityCounts;

		protected InterventionsManager Interventions;

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
			return (uint)Data.GetHash(hashableString);
		}

		public Entity AddEntity(PlayerTeam team, uint playerId, EntityParams ep, Brain br = null)
		{
			uint newId = this.GenerateEntityId(playerId);
			Debug.Log("AddEntity id=" + ep.Id + " " + team + " playerId=" + playerId + " instanceId=" + newId);

			Entity e = new Entity(newId, team, playerId, this.Board, ep, br);
			this.Entites.Add(e);
			this.Entites.Sort(this.EntityComparer);
			this.EntityMap[newId] = e;
			return e;
		}

		public Player AddPlayer(uint id, PlayerTeam team)
		{
			Player p = null;
			if (this.PlayerMap.ContainsKey(id))
			{
				p = this.PlayerMap[id];
				p.Team = team;
				return p;
			}

			p = new Player(id, team);
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
			Debug.Log("HandleGameJoined gameId=" + msg.GameId + " PlayerId=" + msg.PlayerId + " Entities=" + msg.Entities.Length);
			this.Id = msg.GameId;
			this.State = GameState.WaitingForPlayers;

			Player p = this.AddPlayer(msg.PlayerId, msg.Team);
			p.Entities = msg.Entities;

			this.SetDeltaTime(msg.ServerTime);
			this.LoadLevel(msg.LevelId);

			int len = msg.Entities.Length;
			for (int i=0; i<len; i++)
			{
				EntityParams ep = Data.GetEntityData(msg.Entities[i]);
				if (ep != null)
				{
					this.AddEntity(p.Team, p.Id, ep, new CloseAndAttackBrain());
				}
			}
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
