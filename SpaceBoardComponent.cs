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

	public static SpaceBoardComponent Instance;

	public Dictionary<ushort,SpaceBoardNodeComponent> NodeMap;
	public Dictionary<uint,GameBoardEntity> EntityMap;

	public enum InputMode
	{
		None,
		SelectDeployLocation
	}
	public InputMode Input = InputMode.None;
	public GameBoardEntity InputEntity;


	void Awake() 
	{
		Instance = this;
		this.NodeMap = new Dictionary<ushort,SpaceBoardNodeComponent>();
		this.EntityMap = new Dictionary<uint,GameBoardEntity>();
		TacticalBoard.Manager.Init();
		TacticalBoard.Manager.Instance.OnEntityActivity += this.OnEntityActivity;
		TacticalBoard.Manager.Instance.OnGameActivity += this.OnGameActivity;
	}

	public void CreateEntity(TacticalBoard.Entity entity)
	{

		SpaceBoardEntityData data = SpaceBoardEntityManager.Instance.GetEntityData(entity.DataId);
		Debug.LogError("CreateEntity team=" + entity.Team.ToString() + " playerId=" + entity.PlayerId + " id=" + entity.Id + " data=" + data);
		if (data == null)
		{
			return;
		}

		Vector3 pos = new Vector3(-1000, 0, -1000);
		GameObject obj = SpaceBoardEntityManager.Instance.CreateGameBoardEntity(data.Id, pos, Quaternion.identity);
		obj.transform.localScale = new Vector3(data.PrefabScale, data.PrefabScale, data.PrefabScale);

		GameBoardEntity gbe = obj.GetComponent<GameBoardEntity>();
		gbe.Entity = entity;
		obj.name += " " + entity.Id;

		this.EntityMap[entity.Id] = gbe; 
	}

	public void EnableBoardInput()
	{
		foreach(KeyValuePair<ushort, SpaceBoardNodeComponent> pair in this.NodeMap)
		{
			pair.Value.gameObject.SetActive(true);
		}
	}

	public void DisableBoardInput()
	{
		foreach(KeyValuePair<ushort, SpaceBoardNodeComponent> pair in this.NodeMap)
		{
			pair.Value.gameObject.SetActive(false);
		}
	}

	public void BeginDeployEntity(TacticalBoard.Entity entity, TacticalBoard.GridNode node)
	{
		if (!this.EntityMap.ContainsKey(entity.Id))
		{
			return;
		}

		GameBoardEntity gbe = this.EntityMap[entity.Id];
		gbe.BeginDeployTo(node);
	}

	public void CompleteDeployEntity(TacticalBoard.Entity entity, TacticalBoard.GridNode node)
	{
		if (!this.EntityMap.ContainsKey(entity.Id))
		{
			return;
		}

		GameBoardEntity gbe = this.EntityMap[entity.Id];
		gbe.DeployTo(node);
	}

	public void TeleportEntity(TacticalBoard.Entity entity, TacticalBoard.GridNode node)
	{
		if (!this.EntityMap.ContainsKey(entity.Id))
		{
			return;
		}

		GameBoardEntity gbe = this.EntityMap[entity.Id];
		gbe.TeleportTo(node);
	}

	public void DestroyEntity(TacticalBoard.Entity entity)
	{
		if (!this.EntityMap.ContainsKey(entity.Id))
		{
			return;
		}

		GameBoardEntity gbe = this.EntityMap[entity.Id];
		gbe.gameObject.SetActive(false);
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
		TacticalBoard.GridNode node = TacticalBoard.Manager.Instance.CurrentGame.Board.GetNode(x, y);
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

	public void SetInputMode(InputMode mode, GameBoardEntity entity = null)
	{
		switch(mode)
		{
			case InputMode.SelectDeployLocation:
			{
				this.InputEntity = entity;
				this.EnableBoardInput();
				break;
			}
		}

		this.Input = mode;
	}

	public void OnNodeSelected(TacticalBoard.GridNode node)
	{
		switch (this.Input)
		{
			case InputMode.SelectDeployLocation:
			{
				if (this.InputEntity != null)
				{
					this.InputEntity.RequestDeployment(node);
					this.DisableBoardInput();
				}
				break;
			}
		}
	}

	private void CreateBoard(int sizeX, int sizeY)
	{
		Debug.LogError("CreateBoard sizeX=" + sizeX + " sizeY=" + sizeY);

		if (this.BoardNodePrefab == null)
		{
			return;
		}

		this.SizeX = sizeX;
		this.SizeY = sizeY;

		for (int x=0; x<this.SizeX; x++)
		{
			for (int y=0; y<this.SizeY; y++)
			{
				GameObject obj = GameObject.Instantiate(this.BoardNodePrefab, this.transform);
				obj.transform.SetPositionAndRotation(this.GetPos(x, y), Quaternion.identity);

				TacticalBoard.GridNode node = TacticalBoard.Manager.Instance.CurrentGame.Board.GetNode(x, y);
				SpaceBoardNodeComponent nodeComp = obj.GetComponent<SpaceBoardNodeComponent>();
				if ((node != null) && (nodeComp != null))
				{
					nodeComp.Node = node;
					nodeComp.Board = this;
					this.NodeMap.Add(node.Id, nodeComp);
				}
			}
		}

		this.DisableBoardInput();
	}

	private void OnGameActivity(TacticalBoard.Game.ActivityType type, TacticalBoard.Game game, TacticalBoard.Player localPlayer)
	{
		switch (type)
		{
			case TacticalBoard.Game.ActivityType.Join:
			{
				this.CreateBoard(game.Board.X, game.Board.Y);
				break;
			}
		}
	}

	private void OnEntityActivity(List<TacticalBoard.EntityActivity> activity)
	{
		for (int i=0; i<activity.Count; i++)
		{
			TacticalBoard.EntityActivity ea = activity[i];

			switch (ea.Type)
			{
				case TacticalBoard.EntityActivity.ActivityType.Created:
				{
					//Debug.Log("ActivityType.Created " + ea.EntitySource.Id + " " + ea.EntitySource.Current.Id + " ");
					this.CreateEntity(ea.EntitySource);
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.Deploying:
				{
					this.BeginDeployEntity(ea.EntitySource, ea.Location);
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.Deployed:
				{
					Debug.Log("ActivityType.Deployed " + ea.EntitySource.Id + " to " + ea.Location.x + "," + ea.Location.y);
					this.CompleteDeployEntity(ea.EntitySource, ea.Location);
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.Teleported:
				{
					Debug.Log("ActivityType.Teleported " + ea.EntitySource.Id + " to " + ea.Location.x + "," + ea.Location.y);
					this.TeleportEntity(ea.EntitySource, ea.Location);
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.SetCourse:
				{
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.RotatedTo:
				{
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.AttackedEntity:
				{
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.Disabled:
				{
					break;
				}

				case TacticalBoard.EntityActivity.ActivityType.Destroyed:
				{
					this.DestroyEntity(ea.EntitySource);
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		if (TacticalBoard.Manager.Instance != null)
		{
			TacticalBoard.Manager.Instance.Update();
			this.TurnCount = TacticalBoard.Manager.Instance.TurnCount;
		}

	}
}
