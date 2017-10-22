using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class SquareGrid : Grid
	{
		public GridNode [,] Nodes;

		public SquareGrid(int x, int y)
		{
			this.Nodes = new GridNode[x,y];
		}

		public bool Spawn(Entity entity, SpawnType st)
		{
			return true;
		}

		public bool Contains(GridNode n)
		{
			return false;
		}

		public bool IsLinked(GridNode n1, GridNode n2)
		{
			return false;
		}

		public virtual GridNode GetNode(int x, int y)
		{
			return this.Nodes[x,y];
		}
	}
}
