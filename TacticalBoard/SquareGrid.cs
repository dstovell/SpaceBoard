using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class SquareGridSearcher : GridSearcher
	{
		private SquareGrid squareGrid
		{
			get
			{
				return this.ParentGrid as SquareGrid;
			}
		}
		private EpPathFinding.JumpPointParam jumpPointParam;

		public SquareGridSearcher(SquareGrid grid, bool allowEndNodeUnWalkable = true, bool crossCorner = true, bool crossAdjacentPoint = true)
		{
			this.ParentGrid = grid;
			this.jumpPointParam = new EpPathFinding.JumpPointParam(grid.SearchGrid, allowEndNodeUnWalkable, crossCorner, crossAdjacentPoint);
		}

		public override List<GridNode> GetPath(GridNode n1, GridNode n2)
		{
			this.jumpPointParam.SetEndPoints(n1.gridPos, n2.gridPos);
			return squareGrid.TranslateNodes( EpPathFinding.JumpPointFinder.FindPath(this.jumpPointParam) );
		}
	}

	public class SquareGrid : Grid
	{
		public EpPathFinding.BaseGrid SearchGrid;
		public GridNode [,] Nodes;
		public Dictionary<ushort,GridNode> NodeMap;
		public int X;
		public int Y;

		public SquareGrid(int x, int y)
		{
			this.X = x;
			this.Y = y;
			this.Nodes = new GridNode[x,y];
			this.NodeMap = new Dictionary<ushort,GridNode>();
			this.SearchGrid = new EpPathFinding.StaticGrid(x, y);

			ushort nextId = 0;
			for (int dx=0; dx<x; dx++)
			{
				for (int dy=0; dy<y; dy++)
				{
					nextId++;
					GridNode node = new GridNode(nextId, dx, dy);
					this.Nodes[dx,dy] = node;
					this.NodeMap[nextId] = node;
				}
			}
		}

		public override GridSearcher CreateSearcher(bool allowEndNodeUnWalkable = true, bool crossCorner = true, bool crossAdjacentPoint = true)
		{
			return new SquareGridSearcher(this, allowEndNodeUnWalkable, crossCorner, crossAdjacentPoint);
		}

		public override bool Spawn(Entity entity, SpawnType st)
		{
			return true;
		}

		public override bool Contains(GridNode n)
		{
			return false;
		}

		public override bool IsLinked(GridNode n1, GridNode n2)
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

		public override GridNode GetNode(ushort id)
		{
			return this.NodeMap.ContainsKey(id) ? this.NodeMap[id] : null;
		}

		public List<GridNode> TranslateNodes(List<EpPathFinding.GridPos> nodes)
		{
			List<GridNode> outNodes = new List<GridNode>();
			for (int i=0; i<nodes.Count; i++)
			{
				outNodes.Add(this.TranslateNode(nodes[i]));
			}
			return outNodes;
		}

		public GridNode TranslateNode(EpPathFinding.GridPos node)
		{
			return this.GetNode(node.x, node.y);
		}

		public override bool UpdatePathfinding(List<Entity> entities)
		{
			if (this.SearchGrid == null)
			{
				return false;
			}

			//n^2 but should be fast since we have a super small map
			for (int x=0; x<this.X; x++)
			{
				for (int y=0; y<this.Y; y++)
				{
					this.SearchGrid.SetWalkableAt(x, y, true);
				}
			}

			for (int i=0; i<entities.Count; i++)
			{
				Entity e = entities[i];
				this.SearchGrid.SetWalkableAt(e.X, e.Y, false);
			}

			return true;
		}
	}
}
