using UnityEngine;
using System.Collections;

public class SpaceBoardTestLevel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		uint player1Id = 1001;
		TacticalBoard.Player player1 = SpaceBoardComponent.Instance.CreatePlayer(player1Id, TacticalBoard.PlayerTeam.TeamA);
		SpaceBoardComponent.Instance.CreateEntity(player1.Team, player1.Id, "Frigate_Predator_Blue", 0, 0);

		uint player2Id = 1002;
		TacticalBoard.Player player2 = SpaceBoardComponent.Instance.CreatePlayer(player2Id, TacticalBoard.PlayerTeam.TeamB);
		SpaceBoardComponent.Instance.CreateEntity(player2.Team, player2.Id, "Frigate_Predator_Black", 3, 6);
	}
}

