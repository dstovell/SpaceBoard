using UnityEngine;
using System.Collections;

public class GameBoardEntity : MonoBehaviour
{
	public uint EntityId;
	public uint PlayerId;
	public TacticalBoard.EntityParams Params;
	public Sprite CardImage;
	public ShipMover Mover;

	public TacticalBoard.Entity Entity;

	public float move = 1;
	public float range = 1;
	public int attack = 1;
	public int armour = 1;
	public int shield = 1;

	private bool deployed = false;

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

	void Awake()
	{
		this.Mover = this.GetComponent<ShipMover>();
		this.Params = new TacticalBoard.EntityParams();
	}

	void Start()
	{
	}

	public bool RequestDeployment(TacticalBoard.GridNode node)
	{
		if (this.Entity != null)
		{
			return this.Entity.RequestDeployment(node);
		}

		return false;
	}

	public void Activate(Vector3 p)
	{
		if (this.Entity != null)
		{
			this.Entity.ActivateAt(3, 0);
		}
	}

	public bool IsActive()
	{
		return (this.Entity != null) ? this.Entity.IsActive() : false;
	}

	private void SetParams(TacticalBoard.EntityParams p)
	{
		if (p == null)
		{
			return;
		}

		this.Params = p;
		this.move = this.Params.move;
		this.range = this.Params.range;
		this.attack = this.Params.attack;
		this.armour = this.Params.armour;
		this.shield = this.Params.shield;
	}

	void Update ()
	{
		if (this.Entity != null)
		{
			this.EntityId = this.Entity.Id;
			this.SetParams(this.Entity.Current);

			if (!this.deployed && this.Entity.IsDeployed())
			{
				SpaceBoardNodeComponent comp = (this.Entity.Position != null) ? SpaceBoardComponent.Instance.GetNode(this.Entity.Position.Id) : null;
				if (comp != null)
				{
					this.Mover.Warp(comp.transform);
				}
				this.deployed = true;
			}
		}
	}
}

