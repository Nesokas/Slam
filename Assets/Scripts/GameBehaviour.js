#pragma strict

var ball_position = Vector3(0, 0.06968546, 0);

var team_1_inicial_z_position = 7.812522;
var team_2_inicial_z_position = -7.812522;

var num_team_1_players = 1;
var num_team_2_players = 1;

var team_1_prefab : GameObject;
var team_2_prefab : GameObject;
var ball_prefab : GameObject;

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
		Destroy(players_team_1[i] as Object);
	players_team_1.clear();
	
	for (i = 0; i < players_team_2.length; i++)
		Destroy(players_team_2[i] as Object);
	players_team_2.clear();
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
	
	var player_component : PlayerBehaviour;
	
	if (IsOdd(num_team_1_players)) {
	
		position = new Vector3(0, 0.1012379, team_1_inicial_z_position);
		
		for(i = 0; i < num_team_1_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				players_team_1.Push(Instantiate(team_1_prefab, odd_position, team_1_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_1[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
			} else {
				players_team_1.Push(Instantiate(team_1_prefab, position, team_1_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_1[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
				position.x += players_distance;
			}
		}
	} else {
	
		position = new Vector3(0, 0.1012379, team_1_inicial_z_position);
		
		for(i = 0; i < num_team_2_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				players_team_1.Push(Instantiate(team_1_prefab, odd_position, team_1_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_1[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
			} else {
				position.x += players_distance;
				players_team_1.Push(Instantiate(team_1_prefab, position, team_1_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_1[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
			}
		}
	}
	
	if (IsOdd(num_team_2_players)) {

		position = new Vector3(0, 0.1012379, team_2_inicial_z_position);
		
		for(i = 0; i < num_team_2_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				players_team_2.Push(Instantiate(team_2_prefab, odd_position, team_2_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_2[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
			} else {
				players_team_2.Push(Instantiate(team_2_prefab, position, team_2_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_2[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
				position.x += players_distance;
			}
		}
	} else {
		position = new Vector3(0, 0.1012379, team_2_inicial_z_position);
		
		for(i = 0; i < num_team_2_players; i++) {
			if(IsOdd(i)) {
				odd_position = Vector3(-position.x, position.y, position.z);
				players_team_2.Push(Instantiate(team_2_prefab, odd_position, team_2_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_2[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
			} else {
				position.x += players_distance;
				players_team_2.Push(Instantiate(team_2_prefab, position, team_2_prefab.transform.rotation));
				if(keyboard_selected) {
					player = players_team_2[i] as GameObject;
					player_component = player.GetComponent(PlayerBehaviour);
					player_component.AddGamepadNum(gamepad_num++);
				} else keyboard_selected = true;
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