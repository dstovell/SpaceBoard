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

	public void Awake()
	{
		Instance = this;

		this.EntityData = this.gameObject.GetComponents<SpaceBoardEntityData>();
		this.EntityDataMap = new Dictionary<string, SpaceBoardEntityData>();
		for (int i=0; i<this.EntityData.Length; i++)
		{
			SpaceBoardEntityData e = this.EntityData[i];
			this.EntityDataMap[e.Id] = e;
		}
	}

	public SpaceBoardEntityData [] EntityData;
	public Dictionary<string, SpaceBoardEntityData> EntityDataMap;
}

