using UnityEngine;
using System.Collections;

public class Network_Player: Kickoff_Player {
	
	
	private Predictor predictor;
	public NetworkPlayer owner;
	int arrow_id;
	
	new void Start()
	{
		base.Start();
		predictor = new Predictor(transform);	
	}
	
//	Sends player info to every client
	public void InitializePlayerInfo(NetworkPlayer network_player, int team_num, string player_name, Vector3 position, int textureID, int hero_index)
	{
		networkView.RPC("TellInfoToPlayer", RPCMode.All, team_num, player_name, position, network_player, textureID, hero_index);
	}
	
	[RPC]
	void TellInfoToPlayer(int team_num, string name, Vector3 position, NetworkPlayer network_player, int textureID, int hero_index)
	{
		team = team_num;
		
		owner = network_player;

		InstantiateHero(hero_index);
		
		Transform player_mesh = transform.Find("Mesh");
		player_base = player_mesh.Find("Base");
		player_mesh.animation.Play("Idle");

		initial_position = position;
		if(Network.player == network_player) {
			controller_object = (GameObject)Instantiate(player_controller_prefab);
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			player_controller.setInputNum(0);
		}
		
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Network_Game network_game = game_controller.GetComponent<Network_Game>();
		arrow_id = textureID;
        indicator_arrow = network_game.GetTexture(textureID);
	}

	public void GetPlayerInfo()
	{
		networkView.RPC("RPC_GetPlayerInfo", RPCMode.Server, Network.player);
	}
	
	[RPC]
	void RPC_GetPlayerInfo(NetworkPlayer network_player)
	{
		networkView.RPC("GivePlayerInfo", network_player, team, owner, initial_position, transform.position, arrow_id);
	}
	
	[RPC]
	void GivePlayerInfo(int player_team, NetworkPlayer net_player, Vector3 start_position, Vector3 actual_position, int texture_id)
	{
		team = player_team;
		owner = net_player;
		initial_position = start_position;
		animation.Play("Idle");
		
		if(Network.player == net_player) {
			controller_object = (GameObject)Instantiate(player_controller_prefab);
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			player_controller.setInputNum(0);
		}
		
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Network_Game network_game = game_controller.GetComponent<Network_Game>();
		arrow_id = texture_id;
        indicator_arrow = network_game.GetTexture(texture_id);
		
		transform.position = actual_position;
	}
	
	void StopCelebration()
	{
		networkView.RPC("RPC_StopCelebration", RPCMode.All);
	}
	
	[RPC]
	void RPC_StopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	void Update()
	{
		if(Network.isServer){
			if(!ball_collision && commands.shoot != 0) {
				networkView.RPC("UpdateMaterial", RPCMode.All, true);
			} else {
				networkView.RPC("UpdateMaterial", RPCMode.All, false);
			}
		
			networkView.RPC("AskCommands", RPCMode.All);
		} else {
			predictor.PredictPlayer(networkView);
		
			transform.position = predictor.getPredictedTransform().position;
			transform.rigidbody.velocity = predictor.getPredictedTransform().rigidbody.velocity;
		}
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
			networkView.RPC("UpdateCommands", RPCMode.All, commands.horizontal_direction, commands.vertical_direction, commands.shoot, commands.dash, Network.player);
		}
	}
	
	[RPC]
	void UpdateCommands(float horizontal, float vertical, float shoot, float dash, NetworkPlayer network_player)
	{	
		if(network_player == owner && Network.isServer) {
			commands.horizontal_direction = horizontal;
			commands.vertical_direction = vertical;
			commands.shoot = shoot;
			commands.dash = dash;
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
	
	override protected void VerifyPower()
	{
		if (Network.isServer){
			networkView.RPC("UpdateServerPower", RPCMode.Others, commands.dash, commands.horizontal_direction, commands.vertical_direction);
			hero.UsePower(commands);
		}
	}
	
	[RPC]
	void UpdateServerPower(float dash, float horizontal_direction, float vertical_direction)
	{
		commands.dash = dash;
		hero.UsePower(commands);
	}
	
	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		try {
			predictor.OnSerializeNetworkViewPlayer(stream, info);
		} catch (System.Exception ex) {
			predictor = new Predictor(transform);
			predictor.OnSerializeNetworkViewPlayer(stream, info);
		}
	}
	
	public void UpdateCollisions(int scored_team)
	{
		networkView.RPC("RPC_UpdateCollisions", RPCMode.All, scored_team);
	}
	
	[RPC]
	void RPC_UpdateCollisions(int scored_team)
	{
		/*Make a fake notification to just one player*/
		NotificationCenter.Notification notification = new NotificationCenter.Notification(this, "");
		notification.data = new Hashtable();
		notification.data["scored_team"] = scored_team;
		
		DisableGotoCenter(notification);
		InitializePosition();
	}
	
	public void ReleasePlayer()
	{
		networkView.RPC("ReleasePlayers", RPCMode.All);
	}
	
	[RPC]
	new void ReleasePlayers()
	{
		base.ReleasePlayers();
	}
	
	[RPC]
	void EmmitPowerFX(string type = "none")
	{
		hero.EmmitPowerFX(type);
	}
	

}