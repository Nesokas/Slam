using UnityEngine;
using System.Collections;

public class Local_Player : Kickoff_Player {

	public void InitializePlayerInfo(int team_num, string player_name, Vector3 position, int input_num)
	{
		team = team_num;

		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
		name_component.m_camera = (Camera)GameObject.FindGameObjectWithTag("MainCamera").camera;
		name_component.ChangeName(player_name);
		
		animation.Play("Idle");
		initial_position = position;
		controller_object = (GameObject)Instantiate(player_controller_prefab);
		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
		player_controller.setInputNum(input_num);
	}
	
	void StopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	new void Update()
	{
		if(!ball_collision && commands.shoot != 0) {
			player_base.renderer.material = shoot_material;
		} else {
			player_base.renderer.material = normal_material;
		}
		
		UpdateCommands();
		base.Update();
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
