using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* This class is created when a 'main_game' is initiated and is never destroyed */

public class Game_Settings : MonoBehaviour {
	
	public bool local_game;
	public string player_name;
	public bool connected;
	public HostData connect_to;

	public List<Hero_Selection.Player> players_list;

	public int team_1_count = 0;
	public int team_2_count = 0;

	void Awake()
	{
		connected = false;
		DontDestroyOnLoad(this);
		players_list = new List<Hero_Selection.Player>();
	}
	
	public bool IsLocalGame()
	{
		return local_game;
	}
	
	public string PlayerName()
	{
		return player_name;
	}

	public void AddPlayer(Hero_Selection.Player player)
	{
		if (player.team == 1)
			team_1_count++;
		else
			team_2_count++;

		players_list.Add(player);
	}

}
