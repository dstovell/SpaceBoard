using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUI : MonoBehaviour 
{
	public ShipMover Mover;
	public Transform Warper;

	// Use this for initialization
	void Start () 
	{
		
	}

	void OnMouseDown()
	{
		Debug.Log("OnMouseDown " + this.gameObject.name);
		if ((this.Mover != null) && (this.Warper != null))
		{
			this.Mover.Warp(this.Warper);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
