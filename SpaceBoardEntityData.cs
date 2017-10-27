using UnityEngine;
using System.Collections;

public class SpaceBoardEntityData : MonoBehaviour
{
	public string Id;
	public string Name;
	public Sprite CardImage;
	public GameObject Prefab;
	public float PrefabScale = 1.0f;

	public TacticalBoard.EntityParams Params;
	public int move = 1;
	public float range = 1;
	public int attack = 1;
	public int armour = 1;
	public int shield = 1;

	void Awake()
	{
		this.Params = new TacticalBoard.EntityParams();
		this.Params.move = this.move;
		this.Params.range = this.range;
		this.Params.attack = this.attack;
		this.Params.armour = this.armour;
		this.Params.shield = this.shield;
	}
}

