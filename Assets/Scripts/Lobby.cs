using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {
	
	public GUIStyle lobby_style;
	public GUIStyle team1_style;
	public GUIStyle team2_style;
	public GUIStyle names_style;
	public GUIStyle buttons_style;
	public float height_scale = 0.04f;
	public float scale = 0.0025f;
	public float names_scale = 0.04f;
	
	private int num_controllers = 0;
	private float buttons_scale_width;
	
	private List<string> players_team_1 = new List<string>();
	private List<string> players_team_2 = new List<string>();
	private List<string> players_team_0 = new List<string>();
	private List<bool> players_first_loggin = new List<bool>();

	// Use this for initialization
	void Start () 
	{
		buttons_scale_width = (float)Screen.height * scale;
		for(int i = 0; i < 5; i++) {
			players_first_loggin.Add(true);
		}
	}
	
	void AddPlayer(Vector2 position, string player_name, int team)
	{
		names_style.fontSize = Screen.height * (int)names_scale;
		
		if(team == 1){
			position.x = Screen.width/6 + Screen.width/25;
		} else if(team == 2) {
			position.x = 5*Screen.width/6 - Screen.width/25;
		}
		
		if(team == 2 || team == 0){
			if(GUI.Button(new Rect(position.x - 55*buttons_scale_width, 
						       position.y + 5*buttons_scale_width,
						       20*buttons_scale_width,
						       20*buttons_scale_width), 
					      "<")) {
				if(RemoveFromTeam(player_name, 2))
					players_team_0.Add(player_name);
				else if(RemoveFromTeam(player_name, 0))
					players_team_1.Add(player_name);
			}
		}
				   
		GUI.Box(new Rect(position.x - 30*buttons_scale_width,
			 	     position.y + 1*buttons_scale_width,
					 60*buttons_scale_width,
					 30*buttons_scale_width), 
				player_name, 
				names_style);
		
		if(team == 1 || team == 0) {
			if(GUI.Button(new Rect(position.x + 35*buttons_scale_width, 
							position.y + 5*buttons_scale_width,
							20*buttons_scale_width,
							20*buttons_scale_width), ">")) {
				if(RemoveFromTeam(player_name, 1))
					players_team_0.Add(player_name);
				else if(RemoveFromTeam(player_name, 0))
					players_team_2.Add(player_name);
			}
		}
	}
	
	bool RemoveFromTeam(string player_name, int team)
	{
		List<string> team_players;
		if(team == 1)
			team_players = players_team_1;
		else if (team == 2)
			team_players = players_team_2;
		else
			team_players = players_team_0;
			
		for(int i = 0; i < team_players.Count; i++) {
			if(player_name == team_players[i]) {
				team_players.RemoveAt(i);
				return true;
			}
		}
		
		return false;
	}
	
	bool IsPlayerTeam(string player_name, int team)
	{
		List<string> team_players;
		if(team == 1)
			team_players = players_team_1;
		else
			team_players = players_team_2;
			
		for(int i = 0; i < team_players.Count; i++) {
			if(player_name == team_players[i])
				return true;
		}
		
		return false;
	}
	
	void OnGUI () {
	
		float font_scale = height_scale * Screen.height;
		lobby_style.fontSize = (int)font_scale;
		team1_style.fontSize = (int)font_scale;
		team2_style.fontSize = (int)font_scale;
		
		GUI.Box(new Rect(10, 10, Screen.width-20, Screen.height-20), "LOBBY", lobby_style); 
		GUI.Box(new Rect(Screen.width/4 - Screen.width/6, 40, Screen.width/4, Screen.height-60), "TEAM 1", team1_style);
		GUI.Box(new Rect(3*Screen.width/4 - Screen.width/12, 40, Screen.width/4, Screen.height-60), "TEAM 2", team2_style);
		
		if(players_first_loggin[0]) {
			players_first_loggin[0] = false;
			players_team_0.Add("Player 0");	
		}
		
		if(IsPlayerTeam("Player 0", 1))
			AddPlayer(new Vector2(Screen.width/2,Screen.height/6), "Player 0", 1);
		else if (IsPlayerTeam("Player 0", 2))
			AddPlayer(new Vector2(Screen.width/2,Screen.height/6), "Player 0", 2);
		else
			AddPlayer(new Vector2(Screen.width/2,Screen.height/6), "Player 0", 0);
		
		if(num_controllers < Input.GetJoystickNames().Length)
			num_controllers = Input.GetJoystickNames().Length;
	
		for(int  i = 0; i < num_controllers; i++) {
			
			string player_name = "Player " + (i+1);
			
			if(players_first_loggin[i+1]) {
				players_first_loggin[i+1] = false;
				players_team_0.Add(player_name);	
			}
			
			if(IsPlayerTeam(player_name, 1))
				AddPlayer(new Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 1);
			else if (IsPlayerTeam(player_name, 2))
				AddPlayer(new Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 2);
			else
				AddPlayer(new Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 0);
		}
		
		buttons_style.fontSize = (int)font_scale;
		if(GUI.Button(new Rect(Screen.width/2 - 35*buttons_scale_width, 6*Screen.height/7, 70*buttons_scale_width, 25*buttons_scale_width), "START", buttons_style)){
			GameObject game_settings = GameObject.FindGameObjectWithTag("settings");
			game_settings.GetComponent<Game_Settings>().players_team_1 = players_team_1;
			game_settings.GetComponent<Game_Settings>().players_team_2 = players_team_2;
			Application.LoadLevel(2);
		}
		
	}
	
	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
	    {
	        Application.LoadLevel(0);
	    }
	}
}
