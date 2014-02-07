using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network_Game : Game_Behaviour {

	public GameObject chat_prefab;

	Dictionary<NetworkPlayer, bool> players_ready;

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
		if(Network.isServer) {
			ball = (GameObject)Network.Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation, 0);
			ball.transform.name = "Ball";
		}

		Instantiate(chat_prefab);
	}
	
	public void ServerStarGame()
	{
		networkView.RPC("StartGame", RPCMode.All);
	}
	
	[RPC]
	void StartGame()
	{
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
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
