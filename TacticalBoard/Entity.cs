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

		public int move = 1;
		public int attack = 1;
		public int armour = 1;
		public int shield = 1;

		public int x = 3;
		public int y = 0;
	}

	public class Entity
	{
		public EntityParams Initial = null;
		public EntityParams Current = null;
		public Grid ParentGrid = null;
		public GridNode Position = null;
		public bool Activated = false;

		public Entity(Grid grid, EntityParams ep)
		{
			this.Init(grid, ep);
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

		protected void Init(Grid grid, EntityParams ep)
		{
			this.ParentGrid = grid;
			this.Initial = ep;
			Reset();
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

		public bool ActivateAt(int x, int y)
		{
			if (this.Activated)
			{
				return false;
			}

			if (this.ParentGrid != null)
			{
				GridNode n = this.ParentGrid.GetNode(x, y);
				this.MoveTo(n);
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
	}
}
