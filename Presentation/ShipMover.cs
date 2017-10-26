using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipMover : MonoBehaviour
{
	public enum MoveMode
	{
		Idle,
		Warping,
		Moving
	}
	public MoveMode moveMode = MoveMode.Idle;

	public GameBoardEntity Entity;

	public float MoveSpeed = 1f;
	public float RotateSpeed = 1f;
	private Vector3 MoveToTarget = Vector3.zero;

	public GameObject [] Thrusters;

	public float WarpDistance = 1000;
	public float WarpDuration = 3f;
	private float WarpTime = 3f;
	private Vector3 WarpFrom;
	private Vector3 WarpTo;

	private int lastX = -1;
	private int lastY = -1;

	public bool warpComplete = false;

	void Awake()
	{
		this.Entity = this.gameObject.GetComponent<GameBoardEntity>();
	}

	void OnDestroy()
	{
	}

	// Use this for initialization
	void Start() 
	{
	}

	public bool IsDead()
	{
		return false;
	}

	public void MoveTo(Vector3 to)
	{
		this.moveMode = MoveMode.Moving;
		this.MoveToTarget = to;
	}

	public void Stop()
	{
		this.moveMode = MoveMode.Idle;
		this.MoveToTarget = Vector3.zero;
	}

	public void Warp(Vector3 postion, Quaternion rotation) 
	{
		GameObject obj = new GameObject("Warp");
		obj.transform.position = postion;
		obj.transform.rotation = rotation;
		this.Warp(obj.transform);
	}

	public void Warp(Transform to) 
	{
		this.moveMode = MoveMode.Warping;
		this.WarpTo = to.position;
		this.WarpFrom = this.WarpTo - this.WarpDistance * to.forward;
		this.WarpTime = 0f;

		this.transform.position = this.WarpFrom;
		this.transform.rotation = to.rotation;
	}

	public bool IsActive() 
	{
		return ((this.Entity != null) && this.Entity.IsActive() && this.warpComplete);
	}

	void DoneWarpIn() 
	{
		this.Stop();
		this.warpComplete = true;
	}

	void Update() 
	{
		if (this.IsDead())
		{
			//this.StopFiring();
			//this.Stop();
			return;
		}

		if (this.moveMode == MoveMode.Idle)
		{
			if (this.IsActive())
			{
				float x = SpaceBoardComponent.Instance.GetX(this.Entity.X);
				float y = SpaceBoardComponent.Instance.GetY(this.Entity.Y);

				if ((this.lastX != this.Entity.X) || (this.lastY != this.Entity.Y))
				{
					//Debug.Log("Need to move grid from " + this.lastX + " " + this.lastY + " to " + this.Entity.X + " " + this.Entity.Y);
					//Debug.Log("Need to move position from " + this.transform.position.x + " " + this.transform.position.y + " to " + x + " " + y);

					this.lastX = this.Entity.X;
					this.lastY = this.Entity.Y;

					this.MoveTo(SpaceBoardComponent.Instance.GetPos(this.Entity.X, this.Entity.Y));
					return;
				}
			}
		}

		if (this.moveMode == MoveMode.Warping)
		{
			this.WarpTime += Time.deltaTime;

			float t = this.WarpTime / this.WarpDuration;
			t = Mathf.Min(t*t*t*t, 1f);

			this.transform.position = Vector3.Lerp(this.WarpFrom, this.WarpTo, t);

			this.UpdateThrusters();

			float distance = Vector3.Distance(this.transform.position, this.WarpTo);
			if (distance == 0.0f)
			{
				this.DoneWarpIn();
			}

			return;
		}

		if (this.moveMode == MoveMode.Moving)
		{
			float distance = Vector3.Distance(this.transform.position, this.MoveToTarget);
			if (distance == 0.0f)
			{
				this.Stop();
			}

			Vector3 desiredDirection = (this.MoveToTarget - this.transform.position).normalized;
			Quaternion desiredRotation = (desiredDirection != Vector3.zero) ? Quaternion.LookRotation(desiredDirection) : this.transform.rotation;
			this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRotation, this.RotateSpeed);

			this.transform.position = Vector3.MoveTowards(this.transform.position, this.MoveToTarget, this.MoveSpeed);
		}

		this.UpdateThrusters();
		//this.UpdateWeapons();
		//this.ScanForHostiles();
	}

	private void UpdateThrusters()
	{
		for (int i=0; i<this.Thrusters.Length; i++)
		{
			TrailRenderer trail = this.Thrusters[i].GetComponent<TrailRenderer>();
			if (this.moveMode == MoveMode.Warping)
			{
				trail.time = 0.1f;
			}
			else if (this.IsDead())
			{
				trail.time = 0.0f;
			}
			else
			{
				trail.time = 0.3f;
			}
		}
	}
}

