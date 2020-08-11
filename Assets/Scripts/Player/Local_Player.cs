using UnityEngine;
using System.Collections;

public class Local_Player : Kickoff_Player {
	
	public int controller;
	public PlayerController player_controller;

//	void Start()
//	{
//		base.Start();
//		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
//	}

	public void InitializePlayerInfo(int team_num, string player_name, Vector3 position, int input_num, int texture_id, int hero_index)
	{
		team = team_num;

		InstantiateHero(hero_index);

		Transform player_mesh = transform.Find("Mesh");
		player_base = player_mesh.Find("Base");
		player_mesh.GetComponent<Animation>().Play("Idle");
		initial_position = position;
		controller_object = (GameObject)Instantiate(player_controller_prefab);
		player_controller = controller_object.GetComponent<PlayerController>();
		controller = input_num;
		player_controller.setInputNum(input_num);
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Local_Game local_game = game_controller.GetComponent<Local_Game>();
        indicator_arrow = local_game.GetTexture(texture_id);
	}

	void StopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	new void FixedUpdate()
	{
		base.FixedUpdate();
		if(!ball_collision && commands.shoot != 0) {
			player_base.GetComponent<Renderer>().material = shoot_material;
		} else {
			player_base.GetComponent<Renderer>().material = normal_material;
		}

		//if (!is_ai)
		UpdateCommands();

	}

	
	void UpdateCommands()
	{
		commands = player_controller.GetCommands();
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if(team == (int)notification.data["team"]) {
			ChangeAnimation((string)notification.data["reaction"]);
		}
	}
}
