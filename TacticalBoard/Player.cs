using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Player
	{
		public Player(uint id, Hashtable parameters = null)
		{
			this.Id = id;
			this.Parameters = parameters;
		}

		public uint Id;

		Hashtable Parameters;
	}
}

