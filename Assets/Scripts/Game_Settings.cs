using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Settings : MonoBehaviour {
	
	public List<Player> players;
	
	public struct Player
	{
		public int team;
		public string name;
		public Vector3 start_position;
		public NetworkPlayer network_player;
	}

	void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
		players = new List<Player>();
	}
	
	public void AddPlayer(int team, string name, Vector3 start_position, NetworkPlayer network_player)
	{
		Player player = new Player();
		player.team = team;
		player.name = name;
		player.start_position = start_position;
		player.network_player = network_player;
		
		players.Add(player);
	}
}
