using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class EntityAssesment
	{
		public Entity entity;
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
			this.entity = them;
			this.pathTo = us.GetPath(them.Position);
			this.pathDistance = (this.pathTo != null) ? (this.pathTo.Count - 1) : -1;
			this.stepTowards = (this.pathDistance > 0) ? this.pathTo[0] : null;

			float maxRange = us.Current.attackRange;

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
		public Game ParentGame = null;
		public Grid ParentGrid = null;
		public GridNode Position = null;
		public bool Activated = false;
		public long TurnActivated = -1;
		public Brain CurrentBrain = null;
		public GridSearcher Searcher = null;

		public Dictionary<uint,EntityAssesment> Friendlies;
		public Dictionary<uint,EntityAssesment> Hostiles;
		public Dictionary<uint,EntityAssesment> Neutrals;

		public Entity(uint id, PlayerTeam team, uint playerId, Game game, EntityParams ep, Brain br = null)
		{
			this.Id = id;
			this.PlayerId = playerId;
			this.ParentGame = game;
			this.ParentGrid = game.Board;
			this.Initial = ep;
			this.CurrentBrain = br;

			this.Team = team;

			this.Searcher = this.ParentGrid.CreateSearcher();
			this.Friendlies = new Dictionary<uint,EntityAssesment>();
			this.Hostiles = new Dictionary<uint,EntityAssesment>();
			this.Neutrals = new Dictionary<uint,EntityAssesment>();

			this.Reset();
		}

		public int X;

		public int Y;

		public uint DataId
		{
			get
			{
				return (this.Current != null) ? this.Current.Id : 0;
			}
		}

		public long TurnCount
		{
			get
			{
				return (this.ParentGame != null) ? this.ParentGame.TurnCount : 0;
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

		public bool IsShielded()
		{
			return (this.Current.shield > 0);
		}

		public bool IsDead()
		{
			return (this.Current.armour > 0);
		}

		public static int DamageModifier(EntityParams.DamageType damageType, EntityParams.LightType damageLightType, EntityParams.LightType shieldLightType)
		{
			int modifier = 0;

			//No shields
			if (shieldLightType == EntityParams.LightType.None)
			{
				//Light weapons are weak against shields
				if (damageType == EntityParams.DamageType.Light)
				{
					modifier = -1;
				}
				//Explosive weapons do more damage against armour
				else if (damageType == EntityParams.DamageType.Explosive)
				{
					modifier = 1;
				}
			}
			else
			//Shields
			{
				//Light weapons interact differently depending on the shield light type
				if (damageType == EntityParams.DamageType.Light)
				{
					if (shieldLightType == EntityParams.LightType.Blue)
					{
					}
					else if (shieldLightType == EntityParams.LightType.Green)
					{
					}
					else if (shieldLightType == EntityParams.LightType.Purple)
					{
					}
					else if (shieldLightType == EntityParams.LightType.Red)
					{
					}
				}
				//Explosive weapons do less damage against shields
				else if (damageType == EntityParams.DamageType.Explosive)
				{
					modifier = -1;
				}
			}

			return modifier;
		}

		private bool Damage(int amount, EntityParams.DamageType type = EntityParams.DamageType.Kinetic, EntityParams.LightType lightType = EntityParams.LightType.None)
		{
			if (!this.IsActive() || this.IsDead())
			{
				return false;
			}

			int remainingDamage = amount;

			//Shields take damage first
			if (this.IsShielded())
			{
				int actualShieldDamage = remainingDamage + Entity.DamageModifier(type, lightType, this.Current.shieldLightType);
				actualShieldDamage = System.Math.Min(actualShieldDamage, this.Current.shield);

				//If this entity has damageSpillOver ability, allow surplus of un-modified damage to spill over to the armour
				if (this.Current.damageSpillOver)
				{
					remainingDamage = System.Math.Max(0, (remainingDamage - this.Current.shield));
				}
				//If this entity doesn't has damageSpillOver ability, ignore any remaining damage
				else
				{
					remainingDamage = 0;
				}

				this.Current.shield -= actualShieldDamage;
			}
			
			//If there are no shields, or there is a spill over, damage armour
			if (remainingDamage > 0)
			{
				int actualArmourDamage = remainingDamage + Entity.DamageModifier(type, lightType, EntityParams.LightType.None);

				if (actualArmourDamage > this.Current.armour)
				{
					actualArmourDamage = this.Current.armour;
				}

				this.Current.armour -= actualArmourDamage;
			}
			return true;
		}

		private bool OnAttack(Entity attacker)
		{
			if ((attacker == null) || !attacker.IsActive() || !this.IsActive())
			{
				return false;
			}

			EntityParams p = attacker.Current;
			bool didDamage = this.Damage(p.attack, p.attackType, p.attackLightType);

			if (didDamage && this.IsDead())
			{
				this.ParentGame.LogDestroyed(this, attacker);
			}

			return didDamage;
		}

		public bool Attack(Entity target)
		{
			if ((target == null) || (!this.IsActive()))
			{
				return false;
			}

			bool didDamage = target.OnAttack(this);

			this.ParentGame.LogAttackedEntity(this, target);

			return didDamage;
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

			Request r = new Request(this.PlayerId, InterventionType.Deployment, this.TurnCount, this.Id, node.Id);
			r.OnRequestComplete = this.OnDeploymentRequestComplete;
			r.OnRequestAction = this.OnDeployment;

			InterventionsManager.Instance.RequestIntervention(r);

			return true;
		}

		public void OnDeploymentRequestComplete(Request r)
		{
			this.Deployment = (r.Result == ResultType.Success) ? DeploymentState.Deploying : DeploymentState.None;
			if (r.Result == ResultType.Success)
			{
				GridNode node = this.ParentGame.Board.GetNode(r.GridNodeId);
				this.ParentGame.LogEntityDeploying(this, node);
			}
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
					this.ParentGame.LogEntityDeployed(this);
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

		public bool SetCourseTo(List<GridNode> course)
		{
			if ((course == null) || (course.Count == 0))
			{
				return false;
			}

			bool moved = this.MoveTo(course[0]);

			if (moved)
			{
				this.ParentGame.LogSetCourse(this, course);
			}

			return moved;
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
			//TacticalBoard.Debug.Log("Entity.MoveTo " + this.Position.x + "," + this.Position.y);
			if (this.Current != null)
			{
				this.X = this.Position.x;
				this.Y = this.Position.y;
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

		public bool LoadBrainByType<BrainType>() where BrainType : Brain, new()
		{
			Brain b = new BrainType() as Brain;
			if (b != null)
			{
				this.CurrentBrain = b;
				return true;
			}
			return false;
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
