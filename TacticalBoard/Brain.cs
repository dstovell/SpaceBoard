using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public abstract class Brain
	{
		public Brain()
		{
		}

		public bool ThinkMove(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
		{
			return this.OnThinkMove(e, friendlies, hostiles, neutrals);
		}

		public bool ThinkAttack(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
		{
			return this.OnThinkAttack(e, friendlies, hostiles, neutrals);
		}

		public abstract bool OnThinkMove(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals);
		public abstract bool OnThinkAttack(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals);
	}
}

