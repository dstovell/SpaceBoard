using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public delegate void AddEntityDataDelegate(EntityParams ep);

	public static class EntitiesData
	{
		public static void AddEntityData(Dictionary<uint,EntityParams> map, string name, EntityClass.Type type, AddEntityDataDelegate cb)
		{
			EntityParams ep = new EntityParams(name);
			if (Data.Verbose)
			{
				Debug.Log("AddEntityData " + name + " id=" + ep.Id);
			}
			EntityClass.ApplyDefaults(type, ep);
			if (cb != null)
			{
				cb(ep);
			}

			map[ep.Id] = ep;
		}

		public static Dictionary<uint,EntityParams> Init()
		{
			Dictionary<uint,EntityParams> map = new Dictionary<uint,EntityParams>();

			AddEntityData(map, "Asteriod", EntityClass.Type.Inanimate, null);

			AddEntityData(map, "Frigate", EntityClass.Type.Frigate, delegate(EntityParams ep) {
				ep.armour = 5;	
			});

			return map;	
		}
	}
}
