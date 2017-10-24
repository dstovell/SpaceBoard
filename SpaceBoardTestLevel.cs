using UnityEngine;
using System.Collections;

public class SpaceBoardTestLevel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		uint player1 = 1001;
		this.CreatePlayer(player1);
		this.CreateEntity(player1, "Frigate_Predator_Blue", Vector3.zero);

		uint player2 = 1002;
		this.CreatePlayer(player2);
		this.CreateEntity(player1, "Frigate_Predator_Black", new Vector3(10, 0, 0));
	}

	//Move to loader??
	public TacticalBoard.Player CreatePlayer(uint playerId)
	{
		return TacticalBoard.Manager.Instance.AddPlayer(playerId);
	}

	public void CreateEntity(uint playerId, string id, Vector3 pos)
	{
		SpaceBoardEntityData data = SpaceBoardEntityManager.Instance.GetEntityData(id);
		if (data == null)
		{
			return;
		}

		TacticalBoard.ChargeForwardBrain br = new TacticalBoard.ChargeForwardBrain();

		TacticalBoard.Entity e = TacticalBoard.Manager.Instance.AddEntity(playerId, data.Params, br);

		GameObject obj = SpaceBoardEntityManager.Instance.CreateGameBoardEntity(id, pos, Quaternion.identity);
		obj.transform.localScale = new Vector3(data.PrefabScale, data.PrefabScale, data.PrefabScale);

		GameBoardEntity comp = obj.GetComponent<GameBoardEntity>();
		comp.PlayerId = playerId;
		comp.Entity = e;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

