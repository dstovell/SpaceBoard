using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public enum SpawnType
	{
		Teleport = 0,
		Warp
	}

	//TODO: Create SquareGridNode and move pathfinding to there
	public class GridNode : AStar.IPathNode<SquareGridSearcher>
	{
		public GridNode(ushort id, int dx, int dy)
		{
			this.Id = id;
			this.x = dx;
			this.y = dy;
			this.point = new AStar.Point(this.x, this.y);
			this.entitiesIn = new List<Entity>();
		}

		public bool OnEnter(Entity e)
		{
			if (!this.entitiesIn.Contains(e))
			{
				this.entitiesIn.Add(e);
				return true;
			}
			return false;
		}

		public bool OnExit(Entity e)
		{
			if (this.entitiesIn.Contains(e))
			{
				this.entitiesIn.Remove(e);
				return true;
			}
			return false;
		}

		public bool IsWalkable(SquareGridSearcher searcher)
		{
			return (!this.occupied || (this == searcher.To) || (this == searcher.From) || this.entitiesIn.Contains(searcher.EntityMoving));
		}

		public ushort Id;
		public int x;
		public int y;

		public bool occupied
		{
			get
			{
				return (entitiesIn.Count > 0);
			}
		}

		public List<Entity> entitiesIn;

		public AStar.Point point;
	}

	public class GridSearcher
	{
		public Grid ParentGrid;

		//Current Search
		public GridNode From;
		public GridNode To;
		public Entity EntityMoving;

		public virtual List<GridNode> GetPath(GridNode n1, GridNode n2, Entity entityMoving)
		{
			return null;
		}
	}

	public class Grid
	{
		public int X;
		public int Y;

		public virtual GridSearcher CreateSearcher()
		{
			return null;
		}

		public virtual bool UpdatePathfinding(List<Entity> entites)
		{
			return false;
		}

		public virtual bool Spawn(Entity entity, SpawnType st)
		{
			return true;
		}

		public virtual bool Contains(GridNode n)
		{
			return false;
		}

		public virtual bool IsLinked(GridNode n1, GridNode n2)
		{
			return false;
		}

		public virtual GridNode GetNode(int x, int y)
		{
			return null;
		}

		public virtual GridNode GetNode(ushort id)
		{
			return null;
		}
	}
}
