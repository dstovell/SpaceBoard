using UnityEngine;
using System.Collections;

public class SpaceBoardTestLevel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		uint player1 = 1001;
		SpaceBoardComponent.Instance.CreatePlayer(player1);
		SpaceBoardComponent.Instance.CreateEntity(player1, "Frigate_Predator_Blue", Vector3.zero);

		uint player2 = 1002;
		SpaceBoardComponent.Instance.CreatePlayer(player2);
		SpaceBoardComponent.Instance.CreateEntity(player1, "Frigate_Predator_Black", new Vector3(10, 0, 0));
	}
}

