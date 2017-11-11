using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.Curvy;

public class ShipLeader : MonoBehaviour
{
	public SplineController controller;

	public SplineGroup splineGroup;
	public FluffyUnderware.Curvy.CurvySpline spline;

	private float speed = 0f;

	private bool useController = true;

	void Start ()
	{
		this.controller = this.gameObject.GetComponent<SplineController>();
		if (this.controller == null)
		{
			this.controller = this.gameObject.AddComponent<SplineController>();
		}
	}
	
	void Update ()
	{
	}

	public void Pause(float secs)
	{
		if (this.controller != null)
		{		
			this.controller.Pause();
		}
	}

	public void UnPause()
	{
		if ((this.controller != null) && this.controller.IsPaused)
		{		
			this.controller.Play();
		}
	}

	public void SetSpeed(float speed_)
	{
		this.speed = speed_;

		if (this.controller != null)
		{		
			this.controller.Speed = this.speed;
		}
	}

	public void SetTime(float secs_)
	{
		if (this.controller != null)
		{		
			this.controller.TimeScale = secs_;
		}
	}


	public void SetSplineGroup(SplineGroup group)
	{
		this.splineGroup = group;
		this.spline = group.GetClosestSpline(this.transform.position);
		//Debug.LogError("SetSplineGroup " + this.name + " spline=" + spline);
	}

	public Vector3 GetTargetPosition()
	{
		return this.transform.position;
	}

	public void MoveSpline(FluffyUnderware.Curvy.CurvySpline spline_ = null)
	{
		if (spline_ != null)
		{
			this.spline = spline_;
		}

		if ((this.speed == 0) || (this.controller == null) || (this.spline == null))
		{
			return;
		}

		//Debug.LogError("MoveSpline " + this.name + " spline=" + this.spline.name + " speed=" + this.speed);

		Vector3 currentPos = this.transform.position;
		Vector3 nearestPoint;
		float nearestT = this.spline.GetNearestPointTF(currentPos, out nearestPoint);

		if (this.controller.Spline != null)
		{
			float duration = Vector3.Distance(currentPos, nearestPoint) / this.speed;
			this.controller.SwitchTo(this.spline, nearestT, duration);
		}
		else
		{
			this.controller.Spline = this.spline;
			this.controller.InitialPosition = Mathf.Max(nearestT, 0.001f) + 0.01f;
		}

		//Debug.LogError("       currentPos=" + currentPos.ToString() + " nearestPoint=" + nearestPoint.ToString() + " nearestT=" + nearestT);

		this.controller.Play();
	}

	public void ChangeSpline(Vector3 direction, float maxAngle)
	{
		if (this.splineGroup == null)
		{
			return;
		}

		float currentT = this.controller.Position;
		FluffyUnderware.Curvy.CurvySpline newSpline = this.splineGroup.GetChangeSpline(this.spline, currentT, direction, maxAngle);
		if (newSpline == null)
		{
			return;
		}

		this.MoveSpline(newSpline);
	}

	public List<Vector3> GetChangeSplineDirections()
	{
		if ((this.splineGroup == null) || (this.spline == null))
		{
			return new List<Vector3>();
		}

		return this.splineGroup.GetChangeSplineDirections(this.spline, this.controller.Position);
	}

	//Debug this and make it work for spline movement
	public bool IsAtDestination()
	{
		if (this.controller != null)
		{
			return (this.controller.Position == 1.0f);
		}

		return false;
	}

	public bool IsOnCurrentPath(Vector3 pos)
	{
		return false;
	}

	public bool IsMoving()
	{
		if (this.controller != null)
		{
			return this.controller.IsPlaying;
		}

		return false;
	}

	public void Stop()
	{
		if (this.controller != null)
		{
			this.controller.Stop();
		}
	}
}
