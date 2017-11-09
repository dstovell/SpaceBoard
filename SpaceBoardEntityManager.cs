using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceBoardEntityManager : MonoBehaviour
{
	public static SpaceBoardEntityManager Instance = null;

	public GameObject CreateGameBoardEntity(string id, Vector3 pos, Quaternion rot)
	{
		SpaceBoardEntityData e = this.GetEntityData(id);
		if ((e == null) || (e.Prefab == null))
		{
			return null;
		}

		GameObject obj = GameObject.Instantiate(e.Prefab, pos, rot);
		return obj;
	}

	public SpaceBoardEntityData GetEntityData(string id)
	{
		return this.EntityDataMap.ContainsKey(id) ? this.EntityDataMap[id] : null;
	}

	public SpaceBoardEntityData GetEntityData(uint id)
	{
		return this.EntityDataIdMap.ContainsKey(id) ? this.EntityDataIdMap[id] : null;
	}

	public uint GetHash(string s)
	{
		return TacticalBoard.Data.GetHash(s);
	}

	public void Awake()
	{
		Instance = this;

		this.EntityData = this.gameObject.GetComponents<SpaceBoardEntityData>();
		this.EntityDataMap = new Dictionary<string, SpaceBoardEntityData>();
		this.EntityDataIdMap = new Dictionary<uint, SpaceBoardEntityData>();
		for (int i=0; i<this.EntityData.Length; i++)
		{
			SpaceBoardEntityData e = this.EntityData[i];
			this.EntityDataMap[e.Id] = e;
			this.EntityDataIdMap[GetHash(e.Id)] = e;
		}
	}

	public SpaceBoardEntityData [] EntityData;
	public Dictionary<string, SpaceBoardEntityData> EntityDataMap;
	public Dictionary<uint, SpaceBoardEntityData> EntityDataIdMap;
}

