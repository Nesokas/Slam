using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* This class is created when a 'main_game' is initiated and is never destroyed */

public class Game_Settings : MonoBehaviour {
	
	public bool local_game;
	public string player_name;
	public bool connected;
	public HostData connect_to;
	
	void Awake()
	{
		connected = false;
		DontDestroyOnLoad(this);
	}
	
	public bool IsLocalGame()
	{
		return local_game;
	}
	
	public string PlayerName()
	{
		return player_name;
	}
}
