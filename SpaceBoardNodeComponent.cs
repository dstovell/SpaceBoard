using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoardNodeComponent : MonoBehaviour 
{
	public ushort Id
	{
		get
		{
			return (this.Node != null) ? this.Node.Id : (ushort)0;
		}
	}

	public TacticalBoard.GridNode Node;

	void Awake() 
	{
	}
}
		