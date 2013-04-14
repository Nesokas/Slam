#pragma strict

var lobby_style : GUIStyle;
var team1_style : GUIStyle;
var team2_style : GUIStyle;
var names_style : GUIStyle;
var height_scale = 0.04;
var scale = 0.0025;
var names_scale = 0.04;

private var num_controllers = 0;
private var buttons_scale_width : float;

private var players_team_1 = new Array();
private var players_team_2 = new Array();
private var players_team_0 = new Array();
private var players_first_loggin = new Array();

function Start()
{
	for(var i = 0; i < 5; i++) {
		players_first_loggin[i] = true;
	}
}

function AddPlayer(position : Vector2, player_name : String, team : int)
{
	buttons_scale_width = Screen.height * scale;
	names_style.fontSize = Screen.height * names_scale;
	
	if(team == 1){
		position.x = Screen.width/6 + Screen.width/25;
	} else if(team == 2) {
		position.x = 5*Screen.width/6 - Screen.width/25;
	}
	
	if(team == 2 || team == 0){
		if(GUI.Button(Rect(position.x - 55*buttons_scale_width, 
					       position.y + 5*buttons_scale_width,
					       20*buttons_scale_width,
					       20*buttons_scale_width), 
				      "<")) {
			if(removeFromTeam(player_name, 2))
				players_team_0.push(player_name);
			else if(removeFromTeam(player_name, 0))
				players_team_1.push(player_name);
		}
	}
			   
	GUI.Box(Rect(position.x - 30*buttons_scale_width,
		 	     position.y + 1*buttons_scale_width,
				 60*buttons_scale_width,
				 30*buttons_scale_width), 
			player_name, 
			names_style);
	
	if(team == 1 || team == 0) {
		if(GUI.Button(Rect(position.x + 35*buttons_scale_width, 
						position.y + 5*buttons_scale_width,
						20*buttons_scale_width,
						20*buttons_scale_width), ">")) {
			if(removeFromTeam(player_name, 1))
				players_team_0.push(player_name);
			else if(removeFromTeam(player_name, 0))
				players_team_2.push(player_name);
		}
	}
}

function removeFromTeam(player_name : String, team : int)
{
	var team_players : Array;
	if(team == 1)
		team_players = players_team_1;
	else if (team == 2)
		team_players = players_team_2;
	else
		team_players = players_team_0;
		
	for(var i = 0; i < team_players.length; i++) {
		if(player_name == team_players[i]) {
			team_players.splice(i,1);
			return true;
		}
	}
	
	return false;
}

function isPlayerTeam(player_name : String, team : int)
{
	var team_players : Array;
	if(team == 1)
		team_players = players_team_1;
	else
		team_players = players_team_2;
		
	for(var i = 0; i < team_players.length; i++) {
		if(player_name == team_players[i])
			return true;
	}
	
	return false;
}

function OnGUI () {
	
	var font_scale = height_scale * Screen.height;
	lobby_style.fontSize = font_scale;
	team1_style.fontSize = font_scale;
	team2_style.fontSize = font_scale;
	
	GUI.Box(Rect(10, 10, Screen.width-20, Screen.height-20), "LOBBY", lobby_style); 
	GUI.Box(Rect(Screen.width/4 - Screen.width/6, 40, Screen.width/4, Screen.height-60), "TEAM 1", team1_style);
	GUI.Box(Rect(3*Screen.width/4 - Screen.width/12, 40, Screen.width/4, Screen.height-60), "TEAM 2", team2_style);
	
	if(players_first_loggin[0]) {
		players_first_loggin[0] = false;
		players_team_0.push("Player 0");	
	}
	
	if(isPlayerTeam("Player 0", 1))
		AddPlayer(Vector2(Screen.width/2,Screen.height/6), "Player 0", 1);
	else if (isPlayerTeam("Player 0", 2))
		AddPlayer(Vector2(Screen.width/2,Screen.height/6), "Player 0", 2);
	else
		AddPlayer(Vector2(Screen.width/2,Screen.height/6), "Player 0", 0);
	
	if(num_controllers < Input.GetJoystickNames().length)
		num_controllers = Input.GetJoystickNames().length;
	
	for(var i = 0; i < num_controllers; i++) {
		
		var player_name = "Player " + (i+1);
		
		if(players_first_loggin[i+1]) {
			players_first_loggin[i+1] = false;
			players_team_0.push(player_name);	
		}
		
		if(isPlayerTeam(player_name, 1))
			AddPlayer(Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 1);
		else if (isPlayerTeam(player_name, 2))
			AddPlayer(Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 2);
		else
			AddPlayer(Vector2(Screen.width/2,Screen.height/6 + 35*buttons_scale_width*(i+1)), player_name, 0);
	}
	
}
