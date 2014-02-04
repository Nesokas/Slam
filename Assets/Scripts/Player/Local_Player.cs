using UnityEngine;
using System.Collections;

public class Local_Player : Kickoff_Player {
	
	public int controller;

	public void InitializePlayerInfo(int team_num, string player_name, Vector3 position, int input_num, int texture_id, int hero_index)
	{
		team = team_num;

		InstantiateHero(hero_index);

		Transform player_mesh = transform.Find("Mesh");
		player_base = player_mesh.Find("Base");
		player_mesh.animation.Play("Idle");
		initial_position = position;
		controller_object = (GameObject)Instantiate(player_controller_prefab);
		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
		controller = input_num;
		player_controller.setInputNum(input_num);
		
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Local_Game local_game = game_controller.GetComponent<Local_Game>();
        indicator_arrow = local_game.GetTexture(texture_id);
	}

	private void InstantiateHero(int hero_index)
	{
		switch(hero_index) {
		case 0:
			hero = new Sam(this);
			break;
		case 1:
			hero = new Tesla(this, ball);
			break;
		}
		hero.InstantiateMesh(this.transform);

		hero.Start();
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
