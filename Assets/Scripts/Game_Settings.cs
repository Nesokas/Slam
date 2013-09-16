using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Settings : MonoBehaviour {
	
	public List<Player> players;
	public bool local_game;
	
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
		Player player = new Player();
		player.team = team;
		player.name = name;
		player.start_position = start_position;
		player.network_player = network_player;
		player.controller = 0;
		
		players.Add(player);
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
