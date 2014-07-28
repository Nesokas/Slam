using UnityEngine;
using System.Collections;

public class AI : Hero {

	private AIManager ai_manager;
	private PlayerController controller;
	Player_Behaviour player;

	private const int BOTTOM_FLANK = 0, MID_FLANK = 1, TOP_FLANK = 2;

	public AI(Player_Behaviour player)
	{
		hero_prefab = Resources.Load<GameObject>("Heroes/Sam");
		ai_manager = GameObject.Find("AIManager").GetComponent<AIManager>();
		this.player = player;
	}
	// Use this for initialization
	public override void Start () {
		controller = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
		ai_manager.InsertAI(this);
	}
	
	// Update is called once per frame
	public void Update () {
		//int test = GoToArea(1);
		Debug.Log(AreaToFlank(15));
	//	Debug.Log(current_area);
		//controller.commands.horizontal_direction = 1;
		//Debug.Log(controller.commands.horizontal_direction);

	}

	private void GoToArea(int area)
	{

	}

	// given an area, it returns the flank
	private int AreaToFlank(int area) 
	{
		int flank;

		for (int i = 0; i < 6; i++)
			if (area == 3*i)
				return BOTTOM_FLANK;
			else if (area == 3*i+1)
				return MID_FLANK;
			else if (area == 3*i+2)
				return TOP_FLANK;
		return -1;

	}

	public override void UsePower(PlayerController.Commands commands){}

	public override void EmmitPowerFX(string type = "none"){}

}
