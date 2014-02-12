using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Network_Game : Game_Behaviour {

	public GameObject chat_prefab;

	Dictionary<uLink.NetworkPlayer, bool> players_ready;

	protected void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player) 
	{
		uLink.Network.RemoveRPCs(player);
		uLink.Network.DestroyPlayerObjects(player);
		
		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach(GameObject player_obj in all_players){
			Network_Player net_player = player_obj.GetComponent<Network_Player>();
			if (net_player.owner == player) {
				uLink.Network.Destroy(player_obj.GetComponent<uLink.NetworkView>().viewID);
				return;
			}
		}
	}
	
//	protected void uLink_OnDisconnectedFromServer(uLink.NetworkDisconnection info)
//	{
//		uLink.Network.RemoveRPCs(uLink.Network.player);
//		uLink.Network.DestroyPlayerObjects(uLink.Network.player);
//		
//		GameObject[] all_players = GameObject.FindGameObjectsWithTag("Player");
//		
//		foreach(GameObject player_obj in all_players){
//			Network_Player net_player = player_obj.GetComponent<Network_Player>();
//			if (net_player.owner == uLink.Network.player) {
//				uLink.Network.Destroy(player_obj.GetComponent<uLink.NetworkView>().viewID);
//				return;
//			}
//		}
//	}
	
	protected override void MovePlayersToStartPositions()
	{
		if(uLink.Network.isServer) {
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
		if(uLink.Network.isServer) {
			ball = (GameObject)uLink.Network.Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation, 0);
			ball.transform.name = "Ball";
		}

		Instantiate(chat_prefab);
	}
	
	public void ServerStarGame()
	{
		GetComponent<uLink.NetworkView>().RPC("StartGame", uLink.RPCMode.All);
	}
	
	[RPC]
	protected void StartGame()
	{
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
	}
	
	public override void ReleasePlayers()
	{
		if(uLink.Network.isServer) {
			GetComponent<uLink.NetworkView>().RPC("ReleaseClientPlayers", uLink.RPCMode.All);
		}
	}
	
	[RPC]
	public void ReleaseClientPlayers()
	{
		base.ReleasePlayers();
	}
}
