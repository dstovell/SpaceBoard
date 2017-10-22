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

			for (int dx=0; dx<x; dx++)
			{
				for (int dy=0; dy<y; dy++)
				{
					this.Nodes[dx,dy] = new GridNode(dx, dy);
				}
			}
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

		public override GridNode GetNode(int x, int y)
		{
			if ((x >= this.Nodes.GetLength(0)) || (y >= this.Nodes.GetLength(1)))
			{
				return null;
			}

			return this.Nodes[x,y];
		}
	}
}
