using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class EntityParams
	{
		public enum DamageType
		{
			Kinetic,
			Explosive,
			Electrical,
			Light
		}

		public enum LightType
		{
			None,
			Arc,
			Solar,
			Void
		}

		public EntityParams()
		{
		}

		public EntityParams(EntityParams p)
		{
			this.Copy(p);
		}

		public void Copy(EntityParams p)
		{
			this.move = p.move;
			this.range = p.range;
			this.attack = p.attack;
			this.armour = p.armour;
			this.shield = p.shield;

			this.attackType = p.attackType;
			this.attackLightType = p.attackLightType;
			this.sheildLightType = p.sheildLightType;

			this.x = p.x;
			this.y = p.y;
		}

		public int move = 1;
		public float range = 1.0f;
		public int attack = 1;
		public int armour = 1;
		public int shield = 1;

		public DamageType attackType = DamageType.Kinetic;
		public LightType attackLightType = LightType.None;
		public LightType sheildLightType = LightType.None;

		public int x = 0;
		public int y = 0;
	}

	public class EntityAssesment
	{
		public int pathDistance;
		public List<GridNode> pathTo;
		public GridNode stepTowards;
		public float rangeTo;

		public int turnsToReach;
		public int turnsTillRange;

		public int turnsToDefeatShield;
		public int turnsToDefeatArmour;
		public int turnsToKill;

		public bool inRange;
		public bool isHostile;
		public bool isFriendly;

		public int totalTurnsToKill;

		public EntityAssesment()
		{
		}

		public EntityAssesment(Entity us, Entity them)
		{
			this.Assess(us, them);
		}

		public void Assess(Entity us, Entity them)
		{
			this.pathTo = us.GetPath(them.Position);
			this.pathDistance = (this.pathTo != null) ? (this.pathTo.Count - 1) : -1;
			this.stepTowards = (this.pathDistance > 0) ? this.pathTo[0] : null;

			float maxRange = us.Current.range;

			this.rangeTo = Math.Distance(us, them);
			this.turnsToReach = us.TurnsToTravel(this.pathTo);

			this.turnsToDefeatShield = (int)System.Math.Ceiling( (float)them.Current.shield - (float)us.Current.attack );
			this.turnsToDefeatArmour = (int)System.Math.Ceiling( (float)them.Current.armour - (float)us.Current.attack );
			this.turnsToKill = this.turnsToDefeatShield + this.turnsToDefeatArmour;

			this.turnsTillRange = -1;
			if (this.pathTo != null)
			{
				for (int i=0; i<this.pathTo.Count; i++)
				{
					GridNode gn = this.pathTo[i];
					float rangeAt = Math.Distance(gn.x, gn.y, them.X, them.Y);
					if (rangeAt <= maxRange)
					{
						this.turnsTillRange = i + 1;
						break;
					}
				}
			}

			this.totalTurnsToKill = this.turnsTillRange + this.turnsToKill;

			this.isHostile = us.IsTeamHostile(them.Team);
			this.isFriendly = us.IsTeamFriendly(them.Team);
			this.inRange = (this.rangeTo <= maxRange);
		}
	}

	public class Entity
	{
		public enum DeploymentState
		{
			None,
			Deploying,
			Deployed
		}
		public DeploymentState Deployment;

		public uint Id;
		public uint PlayerId;
		public PlayerTeam Team;
		public EntityParams Initial = null;
		public EntityParams Current = null;
		public Grid ParentGrid = null;
		public GridNode Position = null;
		public bool Activated = false;
		public long TurnActivated = -1;
		public Brain CurrentBrain = null;
		public GridSearcher Searcher = null;

		public Dictionary<uint,EntityAssesment> Friendlies;
		public Dictionary<uint,EntityAssesment> Hostiles;
		public Dictionary<uint,EntityAssesment> Neutrals;

		public Entity(uint id, PlayerTeam team, uint playerId, Grid grid, EntityParams ep, Brain br = null)
		{
			this.Id = id;
			this.PlayerId = playerId;
			this.ParentGrid = grid;
			this.Initial = ep;
			this.CurrentBrain = br;

			this.Team = team;

			this.Searcher = this.ParentGrid.CreateSearcher();
			this.Friendlies = new Dictionary<uint,EntityAssesment>();
			this.Hostiles = new Dictionary<uint,EntityAssesment>();
			this.Neutrals = new Dictionary<uint,EntityAssesment>();

			this.Reset();
		}

		public int X
		{
			get
			{
				return (this.Current != null) ? this.Current.x : 0;
			}
		}

		public int Y
		{
			get
			{
				return (this.Current != null) ? this.Current.y : 0;
			}
		}

		public long TurnCount
		{
			get
			{
				return Manager.Instance.TurnCount;
			}
		}

		public List<GridNode> GetPath(GridNode to)
		{
			if (this.Position == null)
			{
				return null;
			}

			return this.Searcher.GetPath(this.Position, to, this);
		}

		public void Reset()
		{
			if (this.Initial != null)
			{
				this.Current = new EntityParams(this.Initial);
			}
		}

		public bool Occupies(GridNode n)
		{
			if ((this.ParentGrid == null) || !this.ParentGrid.Contains(n))
			{
				return false;
			}

			return false;
		}

		public bool IsDamagableOn(GridNode n)
		{
			if (!this.Occupies(n))
			{
				return false;
			}

			return false;
		}

		public bool Damage(int amount, EntityParams.DamageType type = EntityParams.DamageType.Kinetic, EntityParams.LightType lightType = EntityParams.LightType.None)
		{
			return true;
		}

		public bool IsDeployed()
		{
			return (this.Deployment == DeploymentState.Deployed);
		}

		public bool RequestDeployment(int x, int y)
		{
			if (this.ParentGrid == null)
			{
				return false;
			}

			return this.RequestDeployment( this.ParentGrid.GetNode(x, y) );
		}

		public bool RequestDeployment(GridNode node)
		{
			if (this.ParentGrid == null)
			{
				return false;
			}

			if (node == null)
			{
				return false;
			}

			Request r = new Request(InterventionType.Deployment, this.TurnCount, this.Id, node.Id);
			r.OnRequestComplete = this.OnDeploymentRequestComplete;
			r.OnRequestAction = this.OnDeployment;

			InterventionsManager.Instance.RequestIntervention(r);

			return true;
		}

		public void OnDeploymentRequestComplete(Request r)
		{
			this.Deployment = (r.Result == ResultType.Success) ? DeploymentState.Deploying : DeploymentState.None;
		}

		public void OnDeployment(Request r)
		{
			this.Deployment = (r.Result == ResultType.Success) ? DeploymentState.Deployed : DeploymentState.None;
			if (this.Deployment == DeploymentState.Deployed)
			{
				GridNode node = this.ParentGrid.GetNode(r.GridNodeId);
				if (node != null)
				{
					this.ActivateAt(node);
				}
			}
		}

		public bool ActivateAt(int x, int y)
		{
			if (this.ParentGrid != null)
			{
				return this.ActivateAt( this.ParentGrid.GetNode(x, y) );
			}
			return false;
		}

		public bool ActivateAt(GridNode n)
		{
			if (this.Activated)
			{
				return false;
			}

			if (n == null)
			{
				return false;
			}
				
			this.MoveTo(n);
			this.Activated = true;
			this.TurnActivated = this.TurnCount;
			return true;
		}

		public bool MoveTo(GridNode n)
		{
			if (n == null)
			{
				return false;
			}

			if (n.occupied)
			{
				return false;
			}

			if (this.Position != null)
			{
				this.Position.OnExit(this);
			}

			this.Position = n;
			this.Position.OnEnter(this);
			TacticalBoard.Debug.Log("Entity.MoveTo " + this.Position.x + "," + this.Position.y);
			if (this.Current != null)
			{
				this.Current.x = this.Position.x;
				this.Current.y = this.Position.y;
				return true;
			}

			return false;
		}

		public bool IsInitialized()
		{
			return (this.Current != null);
		}

		public bool IsActive()
		{
			return (IsInitialized() && this.Activated && (this.TurnCount > this.TurnActivated));
		}

		public bool CanMove()
		{
			return (this.IsActive() && (this.Current.move > 0));
		}

		public int GetMove()
		{
			if (!this.CanMove())
			{
				return 0;
			}

			return this.Current.move;
		}

		//needs to take in the path later since hard angle transitions will take time
		public int TurnsToTravel(List<GridNode> path)
		{
			if (!this.CanMove() || (path == null))
			{
				return 0;
			}

			int turns = 0;

			for (int i=0; i<path.Count; i++)
			{
				//needs to be based on transtition
				turns++;
			}

			return turns;
		}

		public bool IsTeamHostile(PlayerTeam t) 	{ return ((t != PlayerTeam.Neutral) && (t != this.Team)); }
		public bool IsTeamFriendly(PlayerTeam t) 	{ return ((t != PlayerTeam.Neutral) && (t == this.Team)); }
		public bool IsTeamNeutral(PlayerTeam t) 	{ return (t == PlayerTeam.Neutral); }

		public EntityAssesment GetOrAddAssesment(Entity e)
		{
			if (this.IsTeamHostile(e.Team))
			{
				if (!this.Hostiles.ContainsKey(e.Id))
				{
					this.Hostiles[e.Id] = new EntityAssesment();
				}
				return this.Hostiles[e.Id];
			}
			else if (this.IsTeamFriendly(e.Team))
			{
				if (!this.Friendlies.ContainsKey(e.Id))
				{
					this.Friendlies[e.Id] = new EntityAssesment();
				}
				return this.Friendlies[e.Id];
			}
			else
			{
				if (!this.Neutrals.ContainsKey(e.Id))
				{
					this.Neutrals[e.Id] = new EntityAssesment();
				}
				return this.Neutrals[e.Id];
			}
		}

		public bool UpdateAssements(List<Entity> entites)
		{
			bool updated = false;
			for (int i=0; i<entites.Count; i++)
			{
				Entity e = entites[i];
				if (!e.IsActive() || (e.Id == this.Id))
				{
					continue;
				}

				EntityAssesment assesment = this.GetOrAddAssesment(e);
				assesment.Assess(this, e);
				updated = true;
			}

			return updated;
		}

		public bool UpdateMove()
		{
			bool moved = false;
			if (this.CurrentBrain != null)
			{
				moved = this.CurrentBrain.ThinkMove(this, this.Friendlies, this.Hostiles, this.Neutrals);
			}

			return moved;
		}

		public bool UpdateAttack()
		{
			bool attacked = false;
			if (this.CurrentBrain != null)
			{
				attacked = this.CurrentBrain.ThinkAttack(this, this.Friendlies, this.Hostiles, this.Neutrals);
			}

			return attacked;
		}
	}
}
