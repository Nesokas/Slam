#pragma strict

var ball_position = Vector3(0, 0.06968546, 0);

var team_1_inicial_z_position = 7.812522;
var team_2_inicial_z_position = -7.812522;

var num_team_1_players = 1;
var num_team_2_players = 1;

var team_1_prefab : GameObject;
var team_2_prefab : GameObject;
var player_prefab : GameObject;
var ball_prefab : GameObject;

var m_camera : Camera;
var settings_prefab : GameObject;

private var players_team_1 = new Array();
private var players_team_2 = new Array();
private var ball : GameObject;

private var score_team1 : int = 0;
private var score_team2 : int = 0;

private var players_distance : int = 1.5;
private var gamepad_num = 1;
private var keyboard_selected = false;

function score_team(team:int)
{
	if(team == 1)
		score_team1++;
	else
		score_team2++;
		
	MovePlayersToStartPositions();
}

function OnGUI() 
{
	var score = score_team1 + " - " + score_team2;
	
	GUI.color = Color.black;
	
	var score_style : GUIStyle = GUI.skin.GetStyle("Label");
	score_style.alignment = TextAnchor.UpperCenter;
	score_style.fontSize = 30;
	score_style.fontStyle = FontStyle.Bold;
	GUI.Label(Rect(Screen.width/2, 10, 100, 50), score, score_style);
}

private function IsOdd(num : int)
{
	return num % 2;
}

function destroyAllPlayers() 
{
	var i : int;
	
	for (i = 0; i < players_team_1.length; i++)
		Destroy(players_team_1[i]);
	players_team_1.clear();
	
	for (i = 0; i < players_team_2.length; i++)
		Destroy(players_team_2[i]);
	players_team_2.clear();
}

function GetPlayerNum(player_name : String)
{
	return player_name.Replace("Player ", "");
}

function AddPlayerToTeam(team : int, player : GameObject) 
{
	
	if(team == 1)
		players_team_1.Push(player);
	else
		players_team_2.Push(player);
	
}

function GetPlayer(team : int, position_in_team : int, game_settings : Game_Settings)
{
	if(team == 1)
		return game_settings.players_team_1[position_in_team];
	else
		return game_settings.players_team_2[position_in_team];
}

function InstanciatePlayer(team : int, position : Vector3, team_position : int, game_settings : Game_Settings) {
	
	var player : GameObject = Instantiate(player_prefab, position, player_prefab.transform.rotation);
	AddPlayerToTeam(team, player);
	
	var player_name : String = GetPlayer(team, team_position, game_settings);
	var player_num = parseInt(GetPlayerNum(player_name));
	var player_component = player.GetComponent(PlayerBehaviour);
	
	player_component.InitializePlayerInfo(player_num, team, m_camera);
}

function MovePlayersToStartPositions()
{
	var position : Vector3;
	var i : int;
	var player : GameObject;
	var odd_position : Vector3;
	
	gamepad_num = 1;
	keyboard_selected = false;
	
	destroyAllPlayers();
	
	Destroy(ball);
	ball = Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation);
	ball.transform.name = "Ball";
	
	var settings = GameObject.FindGameObjectWithTag("settings");
	var game_settings : Game_Settings;
	
	if(settings == null) {
		settings = Instantiate(settings_prefab, Vector3(0,0,0), settings_prefab.transform.rotation);
		game_settings = settings.GetComponent(Game_Settings);
		game_settings.AddNewPlayer(1, "Player 0");
		game_settings.AddNewPlayer(1, "Player 1");
	} else {
		game_settings = settings.GetComponent(Game_Settings);
	}
	num_team_1_players = game_settings.players_team_1.length;
	num_team_2_players = game_settings.players_team_2.length;
	
	if (IsOdd(num_team_1_players)) {
	
		position = new Vector3(0, 0.1012379, team_1_inicial_z_position);
		
		for(i = 0; i < num_team_1_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				InstanciatePlayer(1, odd_position, i, game_settings);
			} else {
				InstanciatePlayer(1, position, i, game_settings);
				position.x += players_distance;
			}
		}
	} else {
	
		position = new Vector3(0, 0.1012379, team_1_inicial_z_position);
		
		for(i = 0; i < num_team_1_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				InstanciatePlayer(1, odd_position, i, game_settings);
			} else {
				position.x += players_distance;
				InstanciatePlayer(1, position, i, game_settings);
			}
		}
	}
	
	if (IsOdd(num_team_2_players)) {

		position = new Vector3(0, 0.1012379, team_2_inicial_z_position);
		
		for(i = 0; i < num_team_2_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				InstanciatePlayer(2, odd_position, i, game_settings);
			} else {
				InstanciatePlayer(2, position, i, game_settings);
				position.x += players_distance;
			}
		}
	} else {
		position = new Vector3(0, 0.1012379, team_2_inicial_z_position);
		
		for(i = 0; i < num_team_2_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				InstanciatePlayer(2, odd_position, i, game_settings);
			} else {
				position.x += players_distance;
				InstanciatePlayer(2, position, i, game_settings);
			}
		}
	}
}

function Start () 
{
	
		MovePlayersToStartPositions();
	
}

function Update () {

}