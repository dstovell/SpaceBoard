using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalBoardComponent : MonoBehaviour 
{
	public float SecondsPerTurn = 1.0f;
	public int SizeX = 4;
	public int SizeY = 5;
	public float SizeScale = 10;

	public GameObject BoardNodePrefab;

	private float TimeSinceUpdate = 0.0f;

	private TacticalBoard.Manager Board;

	public static TacticalBoardComponent Instance;

	void Awake() 
	{
		Instance = this;
		TacticalBoard.Manager.Init(this.SizeX, this.SizeY);
		this.Board = TacticalBoard.Manager.Instance;
		CreateBoard();
	}

	public float GetX(int x)
	{
		return (float)(x - this.SizeX/2) * this.SizeScale;
	}

	public float GetY(int y)
	{
		return (float)(y - this.SizeY/2) * this.SizeScale;
	}

	public Vector3 GetPos(int x, int y)
	{
		return new Vector3(this.GetX(x), 0.0f, this.GetY(y));
	}

	void CreateBoard()
	{
		if (this.BoardNodePrefab == null)
		{
			return;
		}

		for (int x=0; x<this.SizeX; x++)
		{
			for (int y=0; y<this.SizeY; y++)
			{
				GameObject obj = GameObject.Instantiate(this.BoardNodePrefab, this.transform);
				obj.transform.SetPositionAndRotation(this.GetPos(x, y), Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		this.TimeSinceUpdate += Time.fixedDeltaTime;

		if (this.TimeSinceUpdate > this.SecondsPerTurn)
		{
			if (TacticalBoard.Manager.Instance != null)
			{
				TacticalBoard.Manager.Instance.Update();
				this.TimeSinceUpdate = 0.0f;
				//Debug.Log("TurnCount=" + this.Board.TurnCount + " " + this.Board.Entites.Count);
			}
		}
	}
}
