using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public long TurnCount = 0;

		public Grid Board;

		public List<Entity> Entites;

		ushort EntityCount = 0;

		public static void Init(int x, int y)
		{
			if (TacticalBoard.Manager.Instance == null)
			{
				TacticalBoard.Manager.Instance = new TacticalBoard.Manager(x, y);
			}
		}

		public void Update()
		{
			this.TurnCount++;

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				e.UpdateMove();
			}

			for (int i=0; i<this.Entites.Count; i++)
			{
				Entity e = this.Entites[i];
				e.UpdateAttack();
			}
		}

		public Entity AddEntity(EntityParams ep, Brain br = null)
		{
			this.EntityCount++;
			Entity e = new Entity(this.EntityCount, this.Board, ep, br);
			this.Entites.Add(e);
			return e;
		}

		public Manager(int x, int y)
		{
			this.TurnCount = 0;
			this.EntityCount = 0;

			this.Board = new SquareGrid(x, y);
			this.Entites = new List<Entity>();
		}
	}
}
