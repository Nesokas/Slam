using UnityEngine;
using System.Collections;

public class Network_Player: Kickoff_Player {
	
	
	private Predictor predictor;
	public uLink.NetworkPlayer owner;
	int arrow_id;
	
	new void Start()
	{
		base.Start();
		predictor = new Predictor(transform);	
	}
	
//	Sends player info to every client
	public void InitializePlayerInfo(uLink.NetworkPlayer network_player, int team_num, string player_name, Vector3 position, int textureID, int hero_index)
	{
		GetComponent<uLink.NetworkView>().RPC("TellInfoToPlayer", uLink.RPCMode.All, team_num, player_name, position, network_player, textureID, hero_index);
	}
	
	[RPC]
	protected void TellInfoToPlayer(int team_num, string name, Vector3 position, uLink.NetworkPlayer network_player, int textureID, int hero_index)
	{
		team = team_num;
		
		owner = network_player;

		InstantiateHero(hero_index);
		
		Transform player_mesh = transform.Find("Mesh");
		player_base = player_mesh.Find("Base");
		player_mesh.animation.Play("Idle");

		initial_position = position;
		if(uLink.Network.player == network_player) {
			controller_object = (GameObject)Instantiate(player_controller_prefab);
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			player_controller.setInputNum(0);
			commands = player_controller.GetCommands();
		}
		
		GameObject game_controller = GameObject.FindGameObjectWithTag("GameController");
        Network_Game network_game = game_controller.GetComponent<Network_Game>();
		arrow_id = textureID;
        indicator_arrow = network_game.GetTexture(textureID);
	}

	public void GetPlayerInfo()
	{
		GetComponent<uLink.NetworkView>().RPC("RPC_GetPlayerInfo", uLink.RPCMode.Server, uLink.Network.player);
	}
	
	[RPC]
	protected void RPC_GetPlayerInfo(uLink.NetworkPlayer network_player)
	{
		GetComponent<uLink.NetworkView>().RPC("GivePlayerInfo", network_player, team, owner, initial_position, transform.position, arrow_id);
	}
	
	[RPC]
	protected void GivePlayerInfo(int player_team, uLink.NetworkPlayer net_player, Vector3 start_position, Vector3 actual_position, int texture_id)
	{
		team = player_team;
		owner = net_player;
		initial_position = start_position;
		animation.Play("Idle");
		
		if(uLink.Network.player == net_player) {
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
		GetComponent<uLink.NetworkView>().RPC("RPC_StopCelebration", uLink.RPCMode.All);
	}
	
	[RPC]
	protected void RPC_StopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	void Update()
	{
		if(uLink.Network.isServer){
			if(!ball_collision && commands.shoot != 0) {
				GetComponent<uLink.NetworkView>().RPC("UpdateMaterial", uLink.RPCMode.All, true);
			} else {
				GetComponent<uLink.NetworkView>().RPC("UpdateMaterial", uLink.RPCMode.All, false);
			}
		
			GetComponent<uLink.NetworkView>().RPC("AskCommands", uLink.RPCMode.All);
		} else {
			predictor.PredictPlayer(GetComponent<uLink.NetworkView>());
		
			transform.position = predictor.getPredictedTransform().position;
			transform.rigidbody.velocity = predictor.getPredictedTransform().rigidbody.velocity;

		}
	}
	
	[RPC] 
	protected void UpdateMaterial(bool shoot)
	{	
		if (shoot)
			player_base.renderer.material = shoot_material;
		else
			player_base.renderer.material = normal_material;
	}
	
	[RPC]
	protected void AskCommands()
	{
		if(uLink.Network.player == owner) {
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			commands = player_controller.GetCommands();
			GetComponent<uLink.NetworkView>().RPC("UpdateCommands", uLink.RPCMode.All, commands.horizontal_direction, commands.vertical_direction, commands.shoot, commands.dash, uLink.Network.player);
		}
	}
	
	[RPC]
	protected void UpdateCommands(float horizontal, float vertical, float shoot, float dash, uLink.NetworkPlayer network_player)
	{	
		if(network_player == owner && uLink.Network.isServer) {
			commands.horizontal_direction = horizontal;
			commands.vertical_direction = vertical;
			commands.shoot = shoot;
			commands.dash = dash;
		}
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if(team == (int)notification.data["team"]) {
			GetComponent<uLink.NetworkView>().RPC("RPCChangeReaction", uLink.RPCMode.All, (string)notification.data["reaction"]);
		}
	}
				
	[RPC]
	protected void RPCChangeReaction(string reaction)
	{
		ChangeAnimation(reaction);
	}
	
	override protected void VerifyPower()
	{
		if (uLink.Network.isServer){
			GetComponent<uLink.NetworkView>().RPC("UpdateServerPower", uLink.RPCMode.Others, commands.dash, commands.horizontal_direction, commands.vertical_direction);
			hero.UsePower(commands);
		}
	}
	
	[RPC]
	protected void UpdateServerPower(float dash, float horizontal_direction, float vertical_direction)
	{
		commands.dash = dash;
		hero.UsePower(commands);
	}
	
	public void uLink_OnSerializeNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
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
		GetComponent<uLink.NetworkView>().RPC("RPC_UpdateCollisions", uLink.RPCMode.All, scored_team);
	}
	
	[RPC]
	protected void RPC_UpdateCollisions(int scored_team)
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
		GetComponent<uLink.NetworkView>().RPC("ReleasePlayers", uLink.RPCMode.All);
	}
	
	[RPC]
	protected new void ReleasePlayers()
	{
		base.ReleasePlayers();
	}
	
	[RPC]
	protected void EmmitPowerFX(string type = "none")
	{
		hero.EmmitPowerFX(type);
	}
	

}