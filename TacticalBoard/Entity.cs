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

		public int x = 1;
		public int y = 1;
	}

	public class Entity
	{
		public EntityParams Initial = null;
		public EntityParams Current = null;
		public Grid ParentGrid = null;
		public GridNode Position = null;

		public Entity(EntityParams ep)
		{
			this.Init(ep);
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

		protected void Init(EntityParams ep)
		{
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

		public bool MoveTo(GridNode n)
		{
			return false;
		}

		public bool IsInitialized()
		{
			return (this.Current != null);
		}

		public bool CanMove()
		{
			return (this.IsInitialized() && (this.Current.move > 0));
		}

		public bool CanMoveTo()
		{
			return (this.IsInitialized() && (this.Current.move > 0));
		}
	}
}
