using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	namespace BrainTypes
	{
		public class CloseAndAttackBrain : Brain
		{
			public override bool OnThinkMove(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
			{
				if (!e.CanMove())
				{
					return false;
				}

				foreach (KeyValuePair<uint,EntityAssesment> pair in hostiles)
				{
					EntityAssesment hostile = pair.Value;
					if (!hostile.inRange && (hostile.stepTowards != null))
					{
						e.MoveTo(hostile.stepTowards);
						break;
					}
				}

				return false;
			}

			public override bool OnThinkAttack(Entity e, Dictionary<uint,EntityAssesment> friendlies, Dictionary<uint,EntityAssesment> hostiles, Dictionary<uint,EntityAssesment> neutrals)
			{
				foreach (KeyValuePair<uint,EntityAssesment> pair in hostiles)
				{
					EntityAssesment hostile = pair.Value;
					if (hostile.inRange)
					{
						//Debug.Log(e.Id + " " + pair.Key + " range=" + hostile.rangeTo + " pathDistance=" + hostile.pathDistance);
						e.Attack(hostile.entity);
						break;
					}
				}

				return false;
			}
		}
	}
}
