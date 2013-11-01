using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* This class is created when a 'main_game' is initiated and is never destroyed */

public class Game_Settings : MonoBehaviour {
	
	public List<Player> players;
	public bool local_game;
	public bool is_game_running = false;
	
	public struct Player
	{
		public int team;
		public string name;
		public Vector3 start_position;
		public NetworkPlayer network_player;
		public int controller;
	}

	void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
		players = new List<Player>();
	}
	
	public void AddNetworkPlayer(int team, string name, Vector3 start_position, NetworkPlayer network_player)
	{
		Player new_player = new Player();
		new_player.team = team;
		new_player.name = name;
		new_player.start_position = start_position;
		new_player.network_player = network_player;
		new_player.controller = 0;
		
		for (int i = 0; i < players.Count; i++) {
			if (players[i].network_player == network_player) {
				
				players[i] = new_player;
				
				networkView.RPC("UpdateNetworkPlayer", RPCMode.All, network_player, team, start_position);
				
				return;
			}	
		}
		players.Add(new_player);
	}
	
	[RPC]
	void UpdateNetworkPlayer(NetworkPlayer network_player, int team, Vector3 start_position)
	{
		GameObject[] players_in_hierarchy = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players_in_hierarchy) {
			Network_Player net_player = (Network_Player) player.GetComponent<Network_Player>();
			
			//net_player.owner in this particular case is the client or server's own player
			if(net_player.owner == network_player) {
				net_player.team = team;
				net_player.initial_position = start_position;
				net_player.Start();
				break;
			}
		}
	}
	
	public void AddLocalPlayer(int team, string name, Vector3 start_position, int controller)
	{
		Player player = new Player();
		player.team = team;
		player.name = name;
		player.start_position = start_position;
		player.controller = controller;
		
		players.Add(player);
	}
	
	public bool IsLocalGame()
	{
		return local_game;
	}
}
