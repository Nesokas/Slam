using UnityEngine;
using System.Collections;

public class Network_Player: Kickoff_Player {

	public void InitializePlayerInfo(NetworkPlayer network_player, int team_num, string player_name, Vector3 position, int textureID)
	{
		networkView.RPC("TellInfoToPlayers", RPCMode.All, team_num, player_name, position, network_player, textureID);
	}
	
	[RPC]
	void TellInfoToPlayers(int team_num, string name, Vector3 position, NetworkPlayer network_player, int textureID)
	{
		team = team_num;
		
		owner = network_player;
//		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
//		name_component.m_camera = (Camera)GameObject.FindGameObjectWithTag("MainCamera").camera;
//		name_component.ChangeName(name);
		
		animation.Play("Idle");
		initial_position = position;
		if(Network.player == network_player) {
			controller_object = (GameObject)Instantiate(player_controller_prefab);
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			player_controller.setInputNum(0);
		}
		
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Network_Game network_game = game_controller.GetComponent<Network_Game>();
        indicator_arrow = network_game.GetTexture(textureID);
	}
	
	void StopCelebration()
	{
		networkView.RPC("RPCStopCelebration", RPCMode.All);
	}
	
	[RPC]
	void RPCStopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	void Update()
	{
		if(!ball_collision && commands.shoot != 0) {
			networkView.RPC("UpdateMaterial", RPCMode.All, true);
		} else {
			networkView.RPC("UpdateMaterial", RPCMode.All, false);
		}
		
		networkView.RPC("AskCommands", RPCMode.All);
	}
	
	[RPC] 
	void UpdateMaterial(bool shoot)
	{	
		if (shoot)
			player_base.renderer.material = shoot_material;
		else
			player_base.renderer.material = normal_material;
	}
	
	[RPC]
	void AskCommands()
	{
		if(Network.player == owner) {
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			commands = player_controller.GetCommands();
			networkView.RPC("UpdateCommands", RPCMode.All, commands.horizontal_direction, commands.vertical_direction, commands.shoot, Network.player);
		}
	}
	
	[RPC]
	void UpdateCommands(float horizontal, float vertical, float shoot, NetworkPlayer network_player)
	{	
		if(network_player == owner && Network.isServer) {
			commands.horizontal_direction = horizontal;
			commands.vertical_direction = vertical;
			commands.shoot = shoot;
		}
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if(team == (int)notification.data["team"]) {
			networkView.RPC("RPCChangeReaction", RPCMode.All, (string)notification.data["reaction"]);
		}
	}
				
	[RPC]
	void RPCChangeReaction(string reaction)
	{
		ChangeAnimation(reaction);
	}
	
	new void Start()
	{
//		if (!networkView.isMine) {	
//			enabled = false;
//		}
		base.Start();
	}
}
