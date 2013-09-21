using UnityEngine;
using System.Collections;

public class Network_Game : Game_Behaviour {

	protected void OnPlayerDisconnected(NetworkPlayer player) 
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	protected void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
	}
	
	protected override void MovePlayersToStartPositions()
	{
		if(Network.isServer) {
			ball.transform.position = ball_position;
			ball.transform.rigidbody.velocity = Vector3.zero;
			if (scored_team != 0) {
				Ball_Behaviour bb = ball.GetComponent<Ball_Behaviour>();
				bb.GameHasRestarted();
			}
		}
		
		Hashtable data = new Hashtable();
		data["scored_team"] = scored_team;
		NotificationCenter.DefaultCenter.PostNotification(this, "DisableGotoCenter", data);
		NotificationCenter.DefaultCenter.PostNotification(this, "InitializePosition");
	}
	
	void Awake()
	{
		if(Network.connections.Length == 0) {
			Network.InitializeServer(32, 8000,false);
		} 
		
		if(Network.isServer) {
			ball = (GameObject)Network.Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation, 0);
			ball.transform.name = "Ball";
			
			GameObject settings =  GameObject.FindGameObjectWithTag("settings");
			
			if(settings == null) {
				GameObject player = (GameObject)Network.Instantiate(player_prefab, new Vector3(0, 0, 7.12416f), transform.rotation, 0);
				Network_Player np = (Network_Player)player.GetComponent<Network_Player>();
				np.InitializePlayerInfo(Network.player, 1, "Test", new Vector3(0, 0, 7.12416f));
			} else {
				Game_Settings game_settings = settings.GetComponent<Game_Settings>();
				for(int i = 0; i < game_settings.players.Count; i++) {
					if(game_settings.players[i].team != 0) {
						GameObject player = (GameObject)Network.Instantiate(player_prefab, game_settings.players[i].start_position, transform.rotation, 0);
						
						Network_Player np = (Network_Player)player.GetComponent<Network_Player>();
						np.InitializePlayerInfo(
							game_settings.players[i].network_player, 
							game_settings.players[i].team, 
							game_settings.players[i].name, 
							game_settings.players[i].start_position
						);
					}
				}
			}
		}
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
	}
}
