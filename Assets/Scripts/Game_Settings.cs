using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Settings : MonoBehaviour {
	
	public List<string> players_team_1;
	public List<string> players_team_2;

	void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
	}
	
	void Start()
	{
		players_team_1 = new List<string>();
		players_team_2 = new List<string>();
	}
	
	public void AddNewPlayer(int team, string name)
	{
		if(team == 1)
			players_team_1.Add(name);
		else
			players_team_2.Add(name);
	}
}
