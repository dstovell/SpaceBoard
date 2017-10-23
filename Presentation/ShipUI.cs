using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUI : MonoBehaviour 
{
	public GameBoardEntity Entity;
	public ShipMover Mover;
	public Transform Warper;

	// Use this for initialization
	void Awake() 
	{
		if (this.Entity == null)
		{
			this.Entity = this.Mover.GetComponent<GameBoardEntity>();	
		}
	}

	void OnMouseDown()
	{
		Debug.Log("OnMouseDown " + this.gameObject.name);
		if (this.Entity != null)
		{
			//this.Entity.RequestDeployment();
		}
		else if ((this.Mover != null) && (this.Warper != null))
		{
			//this.Mover.Warp(this.Warper);

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
