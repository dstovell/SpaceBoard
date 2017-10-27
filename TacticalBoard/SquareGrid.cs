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

		public SquareGridSearcher(SquareGrid grid)
		{
			this.ParentGrid = grid;
		}

		public override List<GridNode> GetPath(GridNode n1, GridNode n2, Entity entityMoving)
		{
			this.From = n1;
			this.To = n2;
			this.EntityMoving = entityMoving;

			LinkedList<GridNode> path = squareGrid.aStar.Search(this.From.point, this.To.point, this);

			this.From = null;
			this.To = null;
			this.EntityMoving = null;

			return squareGrid.TranslateNodes(path);
		}
	}

	public class SquareGrid : Grid
	{
		public AStar.SpatialAStar<GridNode, SquareGridSearcher> aStar;
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

			this.aStar = new AStar.SpatialAStar<GridNode, SquareGridSearcher>(this.Nodes); 
		}

		public override GridSearcher CreateSearcher()
		{
			return new SquareGridSearcher(this);
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

		public List<GridNode> TranslateNodes(LinkedList<GridNode> path, bool excludeStartNode = true)
		{
			List<GridNode> outNodes = new List<GridNode>();
			LinkedListNode<GridNode> front = path.First;
			bool firstNode = true;
			while(front != null)
			{
				if (!excludeStartNode || !firstNode)
				{
					outNodes.Add(front.Value);
				}
				firstNode = false;
        		path.RemoveFirst();
        		front = path.First;
        	}
        	return outNodes;
		}


		public override bool UpdatePathfinding(List<Entity> entities)
		{
			return false;
		}
	}
}
