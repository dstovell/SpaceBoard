using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class EntityActivity
	{
		public enum ActivityType
		{
			Created,
			Deploying,
			Deployed,
			RotatedTo,
			SetCourse,
			AttackedEntity,
			Disabled,
			Destroyed
		}

		//Defined for all types
		public ActivityType Type;
		public Entity EntitySource;
		public GridNode	Location;
		public long Turn;
		public long Time;

		//Defined for RotatedTo
		public GridNode	LookAt;

		//Defined for SetCourse
		public List<GridNode> Course;

		//Defined for AttackedEntity
		public Entity Target;

		//Defined for Disabled, Destroyed
		public Entity Damager;

		public EntityActivity(long turn, ActivityType type, Entity entity, GridNode location = null)
		{
			this.Turn = turn;
			this.Type = type;
			this.EntitySource = entity;
			this.Location = (location != null) ? location : entity.Position;
			Debug.Log("[EntityActivity] " + type + " entity=" + entity.Id);
		}
	}

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
			this.EntityCounts[0] = 0;

			this.Players = new List<Player>();
			this.PlayerMap = new Dictionary<uint,Player>();

			this.Entites = new List<Entity>();
			this.EntityMap = new Dictionary<uint,Entity>();

			this.CurrentActivity = new List<EntityActivity>();
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
				Debug.Log("LoadLevel " + lp.StringId + " " + levelId  + " Placements=" + lp.StaticPlacements.Count);
				this.LevelId = levelId;
				this.CreateBoard(lp);

				for (int i=0; i<lp.StaticPlacements.Count; i++)
				{
					LevelParams.EntityPlacement placement = lp.StaticPlacements[i];
					EntityParams ep = Data.GetEntityData(placement.Id);
					if (ep != null)
					{
						GridNode node = this.Board.GetNode(placement.X, placement.Y);
						if (node != null)
						{
							Entity e = this.AddEntity(placement.Team, 0, ep);
							e.LoadBrainByType<BrainTypes.CloseAndAttackBrain>();
							e.ActivateAt(node);
						}
					}
				}
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

		public uint RemainingPlayerSlots()
		{
			return (uint)System.Math.Max(0, (Game.MaxPlayers - (uint)this.Players.Count));
		}

		public bool IsFull()
		{
			return (this.RemainingPlayerSlots() == 0);
		}

		public void LogEntityCreate(Entity e)
		{
			this.LogEntityBasicActivity(e, EntityActivity.ActivityType.Created);
		}

		public void LogEntityDeploying(Entity e, GridNode deployingAt)
		{
			this.LogEntityBasicActivity(e, EntityActivity.ActivityType.Deploying, deployingAt);
		}

		public void LogEntityDeployed(Entity e)
		{
			this.LogEntityBasicActivity(e, EntityActivity.ActivityType.Deployed);
		}

		public void LogRotateTo(Entity e, GridNode lookAt)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, EntityActivity.ActivityType.RotatedTo, e);
			ea.LookAt = lookAt;
			this.CurrentActivity.Add(ea);
		}

		public void LogSetCourse(Entity e, List<GridNode> course)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, EntityActivity.ActivityType.SetCourse, e);
			ea.Course = course;
			this.CurrentActivity.Add(ea);
		}

		public void LogAttackedEntity(Entity e, Entity target)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, EntityActivity.ActivityType.AttackedEntity, e);
			ea.Target = target;
			this.CurrentActivity.Add(ea);
		}

		public void LogDisabled(Entity e, Entity damager)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, EntityActivity.ActivityType.Disabled, e);
			ea.Damager = damager;
			this.CurrentActivity.Add(ea);
		}

		public void LogDestroyed(Entity e, Entity damager)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, EntityActivity.ActivityType.Destroyed, e);
			ea.Damager = damager;
			this.CurrentActivity.Add(ea);
		}

		protected void LogEntityBasicActivity(Entity e, EntityActivity.ActivityType type, GridNode location = null)
		{
			EntityActivity ea = new EntityActivity(this.TurnCount, type, e, location);
			this.CurrentActivity.Add(ea);
		}

	
		public uint Id;
		public long StartTime { get; protected set; }
		public long EndTime { get; protected set; }
		public long TurnCount { get; protected set; }
		public uint LevelId	{ get; protected set; }
		public Grid Board;
		public List<EntityActivity> CurrentActivity;

		public delegate void EntityActivityDel(List<EntityActivity> activity);		
		public EntityActivityDel OnEntityActivity;

		protected long LastTurnUpdate = 0;

		public List<Entity> Entites;
		public Dictionary<uint, Entity> EntityMap;

		public List<Player> Players;
		public Dictionary<uint, Player> PlayerMap;
		public Player LocalPlayer { get; protected set; }

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
			//First clear state from previous turn
			this.CurrentActivity.Clear();
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


			if (this.OnEntityActivity != null)
			{
				this.OnEntityActivity(this.CurrentActivity);
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

			Entity e = new Entity(newId, team, playerId, this, ep, br);
			this.Entites.Add(e);
			this.Entites.Sort(this.EntityComparer);
			this.EntityMap[newId] = e;
			this.LogEntityCreate(e);
			return e;
		}

		public uint AddPlayerEntities(Player p)
		{
			uint numEtities = 0;
			for (int i=0; i<p.Entities.Length; i++)
			{
				EntityParams ep = Data.GetEntityData(p.Entities[i]);
				if (ep != null)
				{
					Entity e = this.AddEntity(p.Team, p.Id, ep);
					e.LoadBrainByType<BrainTypes.CloseAndAttackBrain>();
					numEtities++;
				}
			}
			return numEtities;
		}

		public Player AddPlayer(uint id, PlayerTeam team, uint [] entites)
		{
			Player p = new Player(id, team);
			p.Entities = entites;
			return this.AddPlayer(p, team);
		}

		protected Player AddPlayer(Player p, PlayerTeam team, bool loadEntites = true)
		{
			if (this.PlayerMap.ContainsKey(p.Id))
			{
				p = this.PlayerMap[p.Id];
				p.Team = team;
				return p;
			}

			p.Team = team;
			p.GameId = this.Id;
			this.Players.Add(p);
			this.Players.Sort(this.PlayerComparer);
			this.PlayerMap[p.Id] = p;
			this.EntityCounts[p.Id] = 0;

			if (loadEntites)
			{
				this.AddPlayerEntities(p);
			}

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

			this.SetDeltaTime(msg.ServerTime);
			this.LoadLevel(msg.LevelId);

			this.LocalPlayer = this.AddPlayer(msg.PlayerId, msg.Team, msg.Entities);
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
			this.AddPlayer(msg.PlayerId, msg.Team, msg.Entities);
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
