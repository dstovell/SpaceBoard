﻿using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Manager 
	{
		public static TacticalBoard.Manager Instance = null;

		public long TurnCount = 0;

		public Grid Board;

		public List<Entity> Entites;

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
		}

		public Entity AddEntity(EntityParams ep)
		{
			Entity e = new Entity(this.Board, ep);
			this.Entites.Add(e);
			return e;
		}

		public Manager(int x, int y)
		{
			this.TurnCount = 0;

			this.Board = new SquareGrid(x, y);
			this.Entites = new List<Entity>();
		}
	}
}
