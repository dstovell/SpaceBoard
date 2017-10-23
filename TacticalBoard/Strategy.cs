using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public abstract class Strategy
	{
		public Strategy()
		{
		}

		public bool ThinkMove(Entity e)
		{
			return this.OnThinkMove(e);
		}

		public bool ThinkAttack(Entity e)
		{
			return this.OnThinkAttack(e);
		}

		public abstract bool OnThinkMove(Entity e);
		public abstract bool OnThinkAttack(Entity e);
	}
}
		