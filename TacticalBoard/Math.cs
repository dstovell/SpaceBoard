using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public static class Math
	{
		public static float Distance(int x1, int y1, int x2, int y2)
		{
			return (float)System.Math.Sqrt( System.Math.Pow(x1-x2, 2) + System.Math.Pow(y1-y2, 2) );
		}

		public static float Distance(Entity us, Entity them)
		{
			return Math.Distance(us.X, us.Y, them.X, them.Y); 
		}
	}
}
