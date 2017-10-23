using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class EntityParams
	{
		public EntityParams()
		{
		}

		public EntityParams(EntityParams p)
		{
			this.move = p.move;
			this.attack = p.attack;
			this.armour = p.armour;
			this.shield = p.shield;

			this.x = p.x;
			this.y = p.y;
		}

		public float move = 1.0f;
		public int attack = 1;
		public int armour = 1;
		public int shield = 1;

		public int x = 0;
		public int y = 0;
	}

	public class Entity
	{
		public ushort Id;
		public EntityParams Initial = null;
		public EntityParams Current = null;
		public Grid ParentGrid = null;
		public GridNode Position = null;
		public bool Activated = false;
		public Brain CurrentBrain = null;

		private float accumulatedMove = 0.0f;

		public Entity(ushort id, Grid grid, EntityParams ep, Brain br = null)
		{
			this.Id = id;
			this.ParentGrid = grid;
			this.Initial = ep;
			this.CurrentBrain = br;
			this.Reset();
			TacticalBoard.Debug.Log("Entity Create " + br);
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

			Request r = new Request(InterventionType.Deployment, Manager.Instance.TurnCount, this.Id, node.Id);
			r.OnRequestComplete = this.OnDeploymentRequestComplete;
			r.OnRequestAction = this.OnDeployment;

			InterventionsManager.Instance.RequestIntervention(r);

			return true;
		}

		public void OnDeploymentRequestComplete(Request r)
		{
		}

		public void OnDeployment(Request r)
		{
		}

		public bool ActivateAt(int x, int y)
		{
			if (this.Activated)
			{
				return false;
			}

			if (this.ParentGrid != null)
			{
				GridNode n = this.ParentGrid.GetNode(x, y);
				if (n != null)
				{
					this.MoveTo(n);
				}
			}

			this.Activated = true;
			return true;
		}

		public bool MoveTo(GridNode n)
		{
			this.Position = n;
			if ((this.Position != null) && (this.Current != null))
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
			return (IsInitialized() && this.Activated);
		}

		public bool CanMove()
		{
			return (this.IsActive() && (this.Current.move > 0));
		}

		public int AvailableMoveAmount()
		{
			if (!this.CanMove())
			{
				return 0;
			}

			return (int)System.Math.Floor(this.accumulatedMove);
		}

		public bool UpdateMove()
		{
			this.accumulatedMove += this.Current.move;
			if (this.accumulatedMove > this.Current.move)
			{
				this.accumulatedMove = this.Current.move;
			}

			bool moved = false;
			if (this.CurrentBrain != null)
			{
				moved = this.CurrentBrain.ThinkMove(this);
			}

			return moved;
		}

		public bool UpdateAttack()
		{
			bool attacked = false;
			if (this.CurrentBrain != null)
			{
				attacked = this.CurrentBrain.ThinkAttack(this);
			}

			return attacked;
		}
	}
}
