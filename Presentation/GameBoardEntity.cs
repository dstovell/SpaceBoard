using UnityEngine;
using System.Collections;

public class GameBoardEntity : MonoBehaviour
{
	public TacticalBoard.EntityParams Params;

	public TacticalBoard.Entity Entity;

	public int move = 1;
	public int attack = 1;
	public int armour = 1;
	public int shield = 1;

	public int X
	{
		get
		{
			return (this.Entity != null) ? this.Entity.X : 0;
		}
	}

	public int Y
	{
		get
		{
			return (this.Entity != null) ? this.Entity.Y : 0;
		}
	}

	// Use this for initialization
	void Start ()
	{
		this.Params = new TacticalBoard.EntityParams();
		this.Params.move = this.move;
		this.Params.attack = this.attack;
		this.Params.armour = this.armour;
		this.Params.shield = this.shield;

		this.Entity = TacticalBoard.Manager.Instance.AddEntity(this.Params);
	}

	public void Activate(Vector3 p)
	{
		if (this.Entity != null)
		{
			this.Entity.ActivateAt(0, 4);
		}
	}

	public bool IsActive()
	{
		return (this.Entity != null) ? this.Entity.IsActive() : false;
	}

	void Update ()
	{
	}
}

