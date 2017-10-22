using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public abstract class Brain
	{
		public Brain()
		{
		}

		public bool Think(Entity e)
		{
			return this.OnThink(e);
		}

		public abstract bool OnThink(Entity e);
	}
}

