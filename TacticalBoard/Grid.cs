using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public enum SpawnType
	{
		Teleport = 0,
		Warp
	}

	public class GridNode
	{
		public GridNode(ushort id, int dx, int dy)
		{
			this.Id = id;
			this.x = dx;
			this.y = dy;
			this.gridPos = new EpPathFinding.GridPos(this.x, this.y);
		}

		public ushort Id;
		public int x;
		public int y;

		public bool occupied;

		public EpPathFinding.GridPos gridPos;
	}

	public class GridSearcher
	{
		public Grid ParentGrid;

		public virtual List<GridNode> GetPath(GridNode n1, GridNode n2)
		{
			return null;
		}
	}

	public class Grid
	{
		public virtual GridSearcher CreateSearcher(bool allowEndNodeUnWalkable = true, bool crossCorner = true, bool crossAdjacentPoint = true)
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
