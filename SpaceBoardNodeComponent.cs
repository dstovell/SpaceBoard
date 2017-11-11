using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoardNodeComponent : MonoBehaviour 
{
	public SpaceBoardComponent Board;

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

	void OnMouseDown()
	{
		if (this.Board != null)
		{
			this.Board.OnNodeSelected(this.Node);
		}
	}
}
		