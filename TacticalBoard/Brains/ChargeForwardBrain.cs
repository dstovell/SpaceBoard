using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class ChargeForwardBrain : Brain
	{
		public override bool OnThinkMove(Entity e)
		{
			if (e.AvailableMoveAmount() > 0)
			{
				int newY = e.Y + e.AvailableMoveAmount();
				GridNode n = e.ParentGrid.GetNode(e.X, newY);
				if (n != null)
				{
					e.MoveTo(n);
					return true;
				}
			}

			return false;
		}

		public override bool OnThinkAttack(Entity e)
		{
			return false;
		}
	}
}
