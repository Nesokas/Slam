using UnityEngine;
using System.Collections;

public class AI : Hero {

	private AIManager ai_manager;
	private PlayerController controller;

	public AI(Player_Behaviour player)
	{
		hero_prefab = Resources.Load<GameObject>("Heroes/Sam");
		ai_manager = GameObject.Find("AIManager").GetComponent<AIManager>();
		player.commands.vertical_direction = 1f;
	}
	// Use this for initialization
	public override void Start () {
	//	GameObject hero = (GameObject)MonoBehaviour.Instantiate(hero_prefab);
		controller = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
		controller.commands.horizontal_direction = 1;
		ai_manager.InsertAI(this);
		//Debug.Log("bot input num: " + player.);
	}
	
	// Update is called once per frame
	public void Update () {


		//controller.commands.horizontal_direction = 1;
		//Debug.Log(controller.commands.horizontal_direction);
	}

	public override void UsePower(PlayerController.Commands commands){}

	public override void EmmitPowerFX(string type = "none"){}

}
