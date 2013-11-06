using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network_Game : Game_Behaviour {
	
	Dictionary<NetworkPlayer, bool> players_ready;

	protected void OnPlayerDisconnected(NetworkPlayer player) 
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		
		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach(GameObject player_obj in all_players){
			Network_Player net_player = player_obj.GetComponent<Network_Player>();
			if (net_player.owner == player) {
				Network.Destroy(player_obj.GetComponent<NetworkView>().viewID);
				return;
			}
		}
	}
	
	protected void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		
		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach(GameObject player_obj in all_players){
			Network_Player net_player = player_obj.GetComponent<Network_Player>();
			if (net_player.owner == Network.player) {
				Network.Destroy(player_obj.GetComponent<NetworkView>().viewID);
				return;
			}
		}
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
//		if(Network.connections.Length == 1) {
//			Network.InitializeServer(32, 8000,false);
//		}
		
		if(Network.isServer) {
			ball = (GameObject)Network.Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation, 0);
			ball.transform.name = "Ball";
			
			GameObject settings =  GameObject.FindGameObjectWithTag("settings");
			
			if(settings == null) {
				GameObject player = (GameObject)Network.Instantiate(player_prefab, new Vector3(0, 0, 7.12416f), transform.rotation, 0);
				Network_Player np = (Network_Player)player.GetComponent<Network_Player>();
				np.InitializePlayerInfo(Network.player, 1, "Test", new Vector3(0, 0, 7.12416f), 0);
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
							game_settings.players[i].start_position,
							i
						);
					}
				}
			}
			
			players_ready = new Dictionary<NetworkPlayer, bool>();
			
			for(int i = 0; i < Network.connections.Length; i++) {
				players_ready.Add(Network.connections[i], false);
			}
		} else {
			networkView.RPC("ClientReady", RPCMode.Server, Network.player);
		}
		
	}
	
	[RPC]
	void ClientReady(NetworkPlayer network_player)
	{
		players_ready[network_player] = true;
		if(AllPlayersReady())
			networkView.RPC("StartGame", RPCMode.All);
	}
	
	bool AllPlayersReady()
	{
		foreach (var player in players_ready) {
			if(!player.Value)
				return false;
		}
		
		return true;
	}
	
	[RPC]
	void StartGame()
	{
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
	}
	
	
	public Texture GetTexture(int id)
	{
		return player_arrows[id];
	}
	
	public override void ReleasePlayers()
	{
		if(Network.isServer) {
			networkView.RPC("ReleaseClientPlayers", RPCMode.All);
		}
	}
	
	[RPC]
	public void ReleaseClientPlayers()
	{
		base.ReleasePlayers();
	}
}
