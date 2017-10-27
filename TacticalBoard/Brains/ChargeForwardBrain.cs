using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class ChargeForwardBrain : Brain
	{
		public override bool OnThinkMove(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
		{
			if (e.GetMove() > 0)
			{
				int newY = e.Y + e.GetMove();
				GridNode n = e.ParentGrid.GetNode(e.X, newY);
				if (n != null)
				{
					e.MoveTo(n);
					return true;
				}
			}

			return false;
		}

		public override bool OnThinkAttack(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
		{
			return false;
		}
	}
}
