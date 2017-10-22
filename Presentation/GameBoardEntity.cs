using UnityEngine;
using System.Collections;

public class GameBoardEntity : MonoBehaviour
{
	public TacticalBoard.EntityParams Params;

	public int move = 1;
	public int attack = 1;
	public int armour = 1;
	public int shield = 1;

	// Use this for initialization
	void Start ()
	{
		this.Params = new TacticalBoard.EntityParams();
		this.Params.move = this.move;
		this.Params.attack = this.attack;
		this.Params.armour = this.armour;
		this.Params.shield = this.shield;

		TacticalBoard.Manager.Instance.AddEntity(this.Params);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

