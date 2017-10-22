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

	void Awake() 
	{
		TacticalBoard.Manager.Init(this.SizeX, this.SizeY);
		this.Board = TacticalBoard.Manager.Instance;
		CreateBoard();
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

				float xPos = (float)(x - this.SizeX/2) * this.SizeScale;
				float zPos = (float)(y - this.SizeY/2) * this.SizeScale;
				obj.transform.SetPositionAndRotation(new Vector3(xPos, 0.0f, zPos), Quaternion.identity);
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
				Debug.Log("TurnCount=" + this.Board.TurnCount + " " + this.Board.Entites.Count);
			}
		}
	}
}
