using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network_Game : Game_Behaviour {

	public GameObject chat_prefab;

	Dictionary<NetworkPlayer, bool> players_ready;
	private Game_Settings game_settings;
	private PhotonView photonView;

//	protected void OnPlayerDisconnected(NetworkPlayer player) 
//	{
//		Network.RemoveRPCs(player);
//		Network.DestroyPlayerObjects(player);
//		
//		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
//		
//		foreach(GameObject player_obj in all_players){
//			Network_Player net_player = player_obj.GetComponent<Network_Player>();
//			if (net_player.owner == player) {
//				Network.Destroy(player_obj.GetComponent<NetworkView>().viewID);
//				return;
//			}
//		}
//	}
	
//	protected void OnDisconnectedFromServer(NetworkDisconnection info)
//	{
//		Network.RemoveRPCs(Network.player);
//		Network.DestroyPlayerObjects(Network.player);
//		
//		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
//		
//		foreach(GameObject player_obj in all_players){
//			Network_Player net_player = player_obj.GetComponent<Network_Player>();
//			if (net_player.owner == Network.player) {
//				Network.Destroy(player_obj.GetComponent<NetworkView>().viewID);
//				return;
//			}
//		}
//	}
	
	protected override void MovePlayersToStartPositions()
	{
		if(game_settings.is_game_creator) {
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
		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = settings.GetComponent<Game_Settings>();

		if(game_settings.is_game_creator) {
			ball = PhotonNetwork.Instantiate("Prefab/Network_Ball", ball_position, ball_prefab.transform.rotation, 0);
			ball.transform.name = "Ball";
		}

		Instantiate(chat_prefab);

		photonView = PhotonView.Get(this);
	}
	
	public void ServerStarGame()
	{
		photonView.RPC("StartGame", PhotonTargets.All);
	}
	
	[RPC]
	void StartGame()
	{
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
	}
	
	public override void ReleasePlayers()
	{
		if(game_settings.is_game_creator) {
			photonView.RPC("ReleaseClientPlayers", PhotonTargets.All);
		}
	}
	
	[RPC]
	public void ReleaseClientPlayers()
	{
		base.ReleasePlayers();
	}
}
