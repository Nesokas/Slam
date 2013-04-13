#pragma strict

var title_style : GUIStyle;
var title_position : Rect;
var team_1_style : GUIStyle;
var team_2_style : GUIStyle;
var string_to_edit = "Player 1";
var controllers : String[];

private var num_players = 2;
private var players_team_1 = new Array();
private var players_team_2 = new Array();
private var selected_controller = new Array();
private var editing = new Array();
private var players_name = new Array();
private var editing_someone = false;

function Start()
{
	players_team_1[0] = 0;
	players_team_2[0] = 1;
	for(var i = 0; i < num_players; i++) {
		selected_controller[i] = "";
		editing[i] = false;
		players_name[i] = "";
	}
}

private function AddPlayer (position : Vector2, player_num)
{		
		players_name[player_num] = GUI.TextField(Rect(position.x, position.y, 100, 20), players_name[player_num], 8);
		if(GUI.Button(Rect(position.x + 120, position.y, 90, 20), selected_controller[player_num] as String) && !editing_someone) {
			editing[player_num] = true;
			editing_someone = true;
		}

		if(editing[player_num]) {
			for (var i = 0; i < controllers.Length; i++) {
				if(GUI.Button(Rect(position.x+120, position.y + 20*(i+1), 90, 20), controllers[i] as String)) {
					selected_controller[player_num] = controllers[i];
					editing[player_num] = false;
					editing_someone = false;
				}
			}
		}
}

function OnGUI () {

	GUI.Box (Rect (10,10,Screen.width-20,Screen.height-20), "");
	GUI.Label(title_position, "Lobby", title_style);
	GUI.Label(Rect(30, 70, 0, 0), "Team 1", team_1_style);
	GUI.Label(Rect(Screen.width-50, 70, 0, 0), "Team 2", team_2_style);
	
	var i : int;
	
	for (i = 0; i < players_team_1.length; i++)
		AddPlayer(Vector2(30, 100 + i*20), players_team_1[i]);
	
	for (i = 0; i < players_team_2.length; i++)
		AddPlayer(Vector2(Screen.width-230, 100 + i*20), players_team_2[i]);
}
