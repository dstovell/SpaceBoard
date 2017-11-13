using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUI : MonoBehaviour 
{
	public GameObject Obj;
	public GameBoardEntity Entity;
	public ShipMover Mover;
	public Transform Warper;

	// Use this for initialization
	void Awake() 
	{
		if (this.Obj != null)
		{
			this.Entity = this.Obj.GetComponent<GameBoardEntity>();
			this.Mover = this.Obj.GetComponent<ShipMover>();
		}
	}

	void OnMouseDown()
	{
		Debug.Log("OnMouseDown " + this.gameObject.name);
		if (this.Entity != null)
		{
			SpaceBoardComponent.Instance.SetInputMode(SpaceBoardComponent.InputMode.SelectDeployLocation, this.Entity);
		}
	}

	void Update () 
	{
		if (this.Entity == null)
		{
			foreach(KeyValuePair<uint,GameBoardEntity> p in SpaceBoardComponent.Instance.EntityMap)
			{
				GameBoardEntity gbe = p.Value;
				if (gbe.Entity.Team == TacticalBoard.PlayerTeam.TeamA)
				{
					this.Entity = gbe;
					break;
				}
			}
		}
	}
}
