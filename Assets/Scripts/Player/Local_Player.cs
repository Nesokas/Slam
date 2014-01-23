using UnityEngine;
using System.Collections;

public class Local_Player : Kickoff_Player {
	
	public int controller;

	public void InitializePlayerInfo(int team_num, string player_name, Vector3 position, int input_num, int texture_id)
	{
		team = team_num;

//		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
//		name_component.m_camera = (Camera)GameObject.FindGameObjectWithTag("MainCamera").camera;
//		name_component.ChangeName(player_name);
		
		animation.Play("Idle");
		initial_position = position;
		controller_object = (GameObject)Instantiate(player_controller_prefab);
		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
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
			player_base.renderer.material = shoot_material;
		} else {
			player_base.renderer.material = normal_material;
		}
		
		UpdateCommands();

	}
	
	void UpdateCommands()
	{
		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
		commands = player_controller.GetCommands();
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if(team == (int)notification.data["team"]) {
			ChangeAnimation((string)notification.data["reaction"]);
		}
	}
}
