﻿using UnityEngine;
using System.Collections;

public class GameBoardEntity : MonoBehaviour
{
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

	public uint EntityId
	{
		get
		{
			return (this.Entity != null) ? this.Entity.Id : 0;
		}
	}

	public uint PlayerId
	{
		get
		{
			return (this.Entity != null) ? this.Entity.PlayerId : 0;
		}
	}

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
		this.Params = new TacticalBoard.EntityParams(this.name);
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

	public bool BeginDeployTo(TacticalBoard.GridNode node)
	{
		SpaceBoardNodeComponent comp = (node != null) ? SpaceBoardComponent.Instance.GetNode(node.Id) : null;
		if (comp != null)
		{
			this.ShowDeployMarker(comp);
			return true;
		}
		return false;
	}

	public bool DeployTo(TacticalBoard.GridNode node)
	{
		if (!this.deployed && this.Entity.IsDeployed())
		{
			SpaceBoardNodeComponent comp = (node != null) ? SpaceBoardComponent.Instance.GetNode(node.Id) : null;
			if (comp != null)
			{
				Quaternion rot = (this.Entity.Team == TacticalBoard.PlayerTeam.TeamA) ? Quaternion.identity : Quaternion.LookRotation(new Vector3(0, 0, -1));

				this.Mover.Warp(comp.transform.position, rot);

				this.deployed = true;
				this.HideDeployMarker();
				return true;
			}
		}

		return false;
	}

	public bool TeleportTo(TacticalBoard.GridNode node)
	{
		SpaceBoardNodeComponent comp = (node != null) ? SpaceBoardComponent.Instance.GetNode(node.Id) : null;
		if (comp != null)
		{
			Quaternion rot = (this.Entity.Team == TacticalBoard.PlayerTeam.TeamA) ? Quaternion.identity : Quaternion.LookRotation(new Vector3(0, 0, -1));

			this.Mover.Teleport(comp.transform.position, rot);

			this.deployed = true;
			return true;
		}

		return false;
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
		this.range = this.Params.attackRange;
		this.attack = this.Params.attack;
		this.armour = this.Params.armour;
		this.shield = this.Params.shield;
	}

	private void ShowDeployMarker(SpaceBoardNodeComponent comp)
	{
	}

	private void HideDeployMarker()
	{
	}

	void Update ()
	{
		if (this.Entity != null)
		{
			this.SetParams(this.Entity.Current);
		}
	}
}

