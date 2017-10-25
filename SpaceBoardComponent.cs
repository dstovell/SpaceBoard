using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoardComponent : MonoBehaviour 
{
	public float SecondsPerTurn = 1.0f;
	public int SizeX = 4;
	public int SizeY = 5;
	public float SizeScale = 10;

	public long TurnCount = 0;

	public GameObject BoardNodePrefab;

	private float TimeSinceUpdate = 0.0f;

	private TacticalBoard.Manager Board;

	public static SpaceBoardComponent Instance;

	public Dictionary<ushort,SpaceBoardNodeComponent> NodeMap;

	void Awake() 
	{
		Instance = this;
		this.NodeMap = new Dictionary<ushort,SpaceBoardNodeComponent>();
		TacticalBoard.Manager.Init(this.SizeX, this.SizeY);
		this.Board = TacticalBoard.Manager.Instance;
		CreateBoard();
	}

	public TacticalBoard.Player CreatePlayer(uint playerId, TacticalBoard.PlayerTeam team)
	{
		return TacticalBoard.Manager.Instance.AddPlayer(playerId, team);
	}

	public void CreateEntity(TacticalBoard.PlayerTeam team, uint playerId, string id, int warpToX, int warpToY)
	{
		SpaceBoardEntityData data = SpaceBoardEntityManager.Instance.GetEntityData(id);
		Debug.Log("CreateEntity team=" + team.ToString() + " playerId=" + playerId + " id=" + id + " warpTo=" + warpToX + "," + warpToY + " data=" + data);
		if (data == null)
		{
			return;
		}

		TacticalBoard.ChargeForwardBrain br = new TacticalBoard.ChargeForwardBrain();

		TacticalBoard.GridNode nodeToWarpTo = TacticalBoard.Manager.Instance.Board.GetNode(warpToX, warpToY);

		TacticalBoard.Entity e = TacticalBoard.Manager.Instance.AddEntity(team, playerId, data.Params, br);

		Vector3 pos = new Vector3(this.GetX(warpToX), 0, this.GetY(warpToY) - 200);
		GameObject obj = SpaceBoardEntityManager.Instance.CreateGameBoardEntity(id, pos, Quaternion.identity);
		obj.transform.localScale = new Vector3(data.PrefabScale, data.PrefabScale, data.PrefabScale);

		GameBoardEntity comp = obj.GetComponent<GameBoardEntity>();
		comp.PlayerId = playerId;
		comp.Entity = e;

		e.RequestDeployment(nodeToWarpTo);
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

	public SpaceBoardNodeComponent GetNode(int x, int y)
	{
		TacticalBoard.GridNode node = TacticalBoard.Manager.Instance.Board.GetNode(x, y);
		if (node == null)
		{
			return null;
		}

		return this.GetNode(node.Id);
	}

	public SpaceBoardNodeComponent GetNode(ushort id)
	{
		return this.NodeMap.ContainsKey(id) ? this.NodeMap[id] : null;
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

				TacticalBoard.GridNode node = TacticalBoard.Manager.Instance.Board.GetNode(x, y);
				SpaceBoardNodeComponent nodeComp = obj.GetComponent<SpaceBoardNodeComponent>();
				if ((node != null) && (nodeComp != null))
				{
					this.NodeMap.Add(node.Id, nodeComp);
				}
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
		this.TurnCount = TacticalBoard.Manager.Instance.TurnCount;
	}
}
