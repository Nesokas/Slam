using UnityEngine;
using System.Collections;

public class Network_Player: Kickoff_Player {
	
	
	private Predictor predictor;
	public PhotonPlayer owner;
	int arrow_id;

	private PhotonView photonView;
	private Game_Settings game_settings;
	
	new void Start()
	{
		base.Start();
		predictor = new Predictor(transform);
		photonView = PhotonView.Get(this);

		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = settings.GetComponent<Game_Settings>();
	}
	
//	Sends player info to every client
	public void InitializePlayerInfo(PhotonPlayer photon_player, int team_num, string player_name, Vector3 position, int textureID, int hero_index)
	{
		photonView.RPC("TellInfoToPlayer", PhotonTargets.All, team_num, player_name, position, photon_player, textureID, hero_index);
	}
	
	[RPC]
	void TellInfoToPlayer(int team_num, string name, Vector3 position, PhotonPlayer photon_player, int textureID, int hero_index)
	{
		team = team_num;
		
		owner = photon_player;

		InstantiateHero(hero_index);
		
		Transform player_mesh = transform.Find("Mesh");
		player_base = player_mesh.Find("Base");
		player_mesh.animation.Play("Idle");

		initial_position = position;
		if(PhotonNetwork.player == photon_player) {
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
		photonView.RPC("RPC_GetPlayerInfo", game_settings.game_creator, PhotonNetwork.player);
	}
	
	[RPC]
	void RPC_GetPlayerInfo(PhotonPlayer photon_player)
	{
		photonView.RPC("GivePlayerInfo", photon_player, team, owner, initial_position, transform.position, arrow_id);
	}
	
	[RPC]
	void GivePlayerInfo(int player_team, PhotonPlayer photon_player, Vector3 start_position, Vector3 actual_position, int texture_id)
	{
		team = player_team;
		owner = photon_player;
		initial_position = start_position;
		animation.Play("Idle");
		
		if(PhotonNetwork.player == photon_player) {
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
		photonView.RPC("RPC_StopCelebration", PhotonTargets.All);
	}
	
	[RPC]
	void RPC_StopCelebration()
	{
		ChangeAnimation("Idle");
	}
	
	void Update()
	{
		if(game_settings.is_game_creator){
			if(!ball_collision && commands.shoot != 0) {
				photonView.RPC("UpdateMaterial", PhotonTargets.All, true);
			} else {
				photonView.RPC("UpdateMaterial", PhotonTargets.All, false);
			}
		
			photonView.RPC("AskCommands", PhotonTargets.All);
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
		if(PhotonNetwork.player == owner) {
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			commands = player_controller.GetCommands();
			photonView.RPC("UpdateCommands", PhotonTargets.All, commands.horizontal_direction, commands.vertical_direction, commands.shoot, commands.dash, PhotonNetwork.player);
		}
	}
	
	[RPC]
	void UpdateCommands(float horizontal, float vertical, float shoot, float dash, PhotonPlayer photon_player)
	{	
		if(photon_player == owner && game_settings.is_game_creator) {
			commands.horizontal_direction = horizontal;
			commands.vertical_direction = vertical;
			commands.shoot = shoot;
			commands.dash = dash;
		}
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if(team == (int)notification.data["team"]) {
			photonView.RPC("RPCChangeReaction", PhotonTargets.All, (string)notification.data["reaction"]);
		}
	}
				
	[RPC]
	void RPCChangeReaction(string reaction)
	{
		ChangeAnimation(reaction);
	}
	
	override protected void VerifyPower()
	{
		if (game_settings.is_game_creator){
			photonView.RPC("UpdateServerPower", PhotonTargets.Others, commands.dash, commands.horizontal_direction, commands.vertical_direction);
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
		photonView.RPC("RPC_UpdateCollisions", PhotonTargets.All, scored_team);
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
		photonView.RPC("ReleasePlayers", PhotonTargets.All);
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