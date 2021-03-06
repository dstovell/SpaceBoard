﻿using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public delegate void AddLevelDataDelegate(LevelParams ep);

	public static class LevelsData
	{
		public static void AddLevelData(Dictionary<uint,LevelParams> map, string name, AddLevelDataDelegate cb)
		{
			LevelParams lp = new LevelParams(name, Data.DefaultSizeX, Data.DefaultSizeY);
			if (Data.Verbose)
			{
				Debug.Log("AddLevelData " + name + " id=" + lp.Id);
			}
			if (cb != null)
			{
				cb(lp);
			}

			map[lp.Id] = lp;
		}

		public static Dictionary<uint,LevelParams> Init()
		{
			Dictionary<uint,LevelParams> map = new Dictionary<uint,LevelParams>();

			AddLevelData(map, "Test01", delegate(LevelParams lp) {
				string entity = "Frigate_Predator_Black";
				lp.AddPlacement(Data.GetHash(entity), 3, 6, PlayerTeam.TeamB);
				lp.AddPlacement(Data.GetHash(entity), 0, 6, PlayerTeam.TeamB);
			});

			return map;	
		}
	}
}
